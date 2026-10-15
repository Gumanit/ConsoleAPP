<#
.SYNOPSIS
    Устанавливает TeamCity test-logger, который нужен плагину C# (OpenIDE Pro / Rider)
    для запуска тестов .NET из IDE.

.DESCRIPTION
    Плагин C# перед запуском тестов сам скачивает пакет TeamCity.VSTest.TestAdapter
    и распаковывает из него TeamCity.VSTest.TestLogger.dll в каталог

        %USERPROFILE%\.openide-dotnet-tools\teamcity-logger      (Windows)
        ~/.openide-dotnet-tools/teamcity-logger                  (Linux / macOS)

    Если скачивание из IDE не проходит (прокси, корпоративный firewall, сбой плагина),
    тесты падают с сообщением "Failed to install test logger" и не запускаются.
    Этот скрипт делает то же самое вручную и кладёт DLL куда нужно.

.PARAMETER Version
    Версия пакета. Должна совпадать с той, что ждёт плагин (сейчас 1.0.41).

.PARAMETER ToolsRoot
    Каталог инструментов плагина. По умолчанию %USERPROFILE%\.openide-dotnet-tools.

.PARAMETER Force
    Переустановить, даже если логгер уже на месте.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\install-test-logger.ps1

.EXAMPLE
    # Проверка на другом каталоге (не трогая рабочую установку)
    powershell -ExecutionPolicy Bypass -File tools\install-test-logger.ps1 -ToolsRoot $env:TEMP\logger-test
#>
[CmdletBinding()]
param(
    [string] $Version = '1.0.41',
    [string] $PackageId = 'teamcity.vstest.testadapter',
    [string] $FeedBaseUrl = 'https://api.nuget.org/v3-flatcontainer',
    [string] $ToolsRoot = (Join-Path $HOME '.openide-dotnet-tools'),
    [switch] $Force
)

$ErrorActionPreference = 'Stop'

$packageLower = $PackageId.ToLowerInvariant()
$targetDir = Join-Path $ToolsRoot 'teamcity-logger'
$loggerDll = Join-Path $targetDir 'TeamCity.VSTest.TestLogger.dll'

if ((Test-Path $loggerDll) -and -not $Force) {
    Write-Host "Логгер уже установлен:" -ForegroundColor Green
    Write-Host "  $loggerDll"
    Write-Host "Переустановить: добавьте -Force"
    exit 0
}

Write-Host "Каталог установки: $targetDir"
New-Item -ItemType Directory -Force -Path $targetDir | Out-Null

$uri = "$FeedBaseUrl/$packageLower/$Version/$packageLower.$Version.nupkg"
$archive = Join-Path ([IO.Path]::GetTempPath()) "$packageLower.$Version.nupkg"

Write-Host "Скачиваю $uri"
try {
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri $uri -OutFile $archive -UseBasicParsing
} catch {
    Write-Host "Не удалось скачать пакет: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Если с этой машины нет доступа к api.nuget.org (прокси/firewall),"
    Write-Host "просто скопируйте каталог с машины, где тесты в IDE работают:"
    Write-Host "    %USERPROFILE%\.openide-dotnet-tools\teamcity-logger"
    Write-Host "в такой же путь на этой машине."
    exit 1
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::OpenRead($archive)
$copied = @()
try {
    # В пакете несколько наборов: build/_common/vstestNN/. Берём самый старший.
    $group = $zip.Entries |
        Where-Object { $_.FullName -match '^build/_common/vstest\d+/[^/]+\.dll$' } |
        ForEach-Object { ($_.FullName -split '/')[2] } |
        Sort-Object -Unique |
        Sort-Object { [int]($_ -replace '\D', '') } |
        Select-Object -Last 1

    if (-not $group) { throw "В пакете нет сборок build/_common/vstestNN/ — изменилась структура пакета." }

    foreach ($entry in $zip.Entries) {
        if ($entry.FullName -like "build/_common/$group/*" -and $entry.Name -ne '') {
            $dest = Join-Path $targetDir $entry.Name
            [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $dest, $true)
            $copied += $entry.Name
        }
    }
} finally {
    $zip.Dispose()
    Remove-Item -LiteralPath $archive -Force -ErrorAction SilentlyContinue
}

if (-not (Test-Path $loggerDll)) {
    throw "TeamCity.VSTest.TestLogger.dll не найден после распаковки (набор $group, файлы: $($copied -join ', '))."
}

Write-Host "Готово. Установлено в $targetDir :" -ForegroundColor Green
$copied | Sort-Object | ForEach-Object { Write-Host "  $_" }
Write-Host ""
Write-Host "Теперь в IDE запускайте конфигурации 'Tests: ...' кнопкой Run."
