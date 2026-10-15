<#
.SYNOPSIS
    Генерирует тестовые данные для приложения «Менеджер задач» в контейнере PostgreSQL.

.DESCRIPTION
    Поднимает контейнер (если он не запущен), выполняет scripts/seed-test-data.sql
    в базе приложения и печатает сводку.

.PARAMETER Count
    Сколько задач сгенерировать (по умолчанию 50).

.PARAMETER Reset
    Перед генерацией очистить таблицы tasks и task_tags (справочники не трогаются).

.PARAMETER Container
    Имя контейнера (по умолчанию todo-postgres).

.EXAMPLE
    powershell -File scripts/seed-test-data.ps1
    powershell -File scripts/seed-test-data.ps1 -Count 200
    powershell -File scripts/seed-test-data.ps1 -Count 200 -Reset
#>
param(
    [int]$Count = 50,
    [switch]$Reset,
    [string]$Container = "todo-postgres",
    [string]$User = "todo",
    [string]$Database = "todo_db"
)

$ErrorActionPreference = "Stop"

$sqlPath = Join-Path $PSScriptRoot "seed-test-data.sql"
if (-not (Test-Path -LiteralPath $sqlPath)) {
    throw "Не найден файл: $sqlPath"
}

# 1. Убедиться, что контейнер запущен
$running = docker ps --filter "name=$Container" --format "{{.Names}}"
if ($running -notcontains $Container) {
    $composeFile = Join-Path (Split-Path $PSScriptRoot -Parent) "docker-compose.yml"
    Write-Host "Контейнер '$Container' не запущен. Поднимаю docker compose -f '$composeFile' up -d ..."
    docker compose -f $composeFile up -d
    for ($i = 0; $i -lt 30; $i++) {
        $health = docker inspect --format "{{.State.Health.Status}}" $Container 2>$null
        if ($health -eq "healthy") { break }
        Start-Sleep -Seconds 2
    }
    if ($health -ne "healthy") {
        Write-Warning "Контейнер пока не 'healthy' (текущее состояние: $health). Продолжаю."
    }
}

# 2. При необходимости очистить данные
if ($Reset) {
    Write-Host "Очистка таблиц tasks и task_tags ..."
    docker exec $Container psql -U $User -d $Database -v ON_ERROR_STOP=1 -c "TRUNCATE TABLE task_tags, tasks;"
}

# 3. Подставить количество задач в SQL
$sql = (Get-Content -LiteralPath $sqlPath -Raw -Encoding UTF8) -replace 'v_count\s+int\s*:=\s*\d+', "v_count      int := $Count"

# 4. Выполнить SQL внутри контейнера (через временный файл — надёжно с UTF-8)
$tempSql = Join-Path $env:TEMP ("seed_" + [guid]::NewGuid().ToString("N") + ".sql")
[System.IO.File]::WriteAllText($tempSql, $sql, (New-Object System.Text.UTF8Encoding($false)))

try {
    docker cp $tempSql "${Container}:/tmp/seed-test-data.sql" | Out-Null
    docker exec $Container psql -U $User -d $Database -v ON_ERROR_STOP=1 -f /tmp/seed-test-data.sql
    docker exec $Container rm -f /tmp/seed-test-data.sql | Out-Null
}
finally {
    Remove-Item -LiteralPath $tempSql -Force -ErrorAction SilentlyContinue
}

Write-Host "Готово: запрошено $Count задач."
