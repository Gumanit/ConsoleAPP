# Менеджер задач (TODO) — консольное приложение на C#

Демонстрационное решение для проверки возможностей разработки на **OpenIDE Pro**. Это
настоящее консольное приложение: задачи, категории, теги, приоритеты, сроки, статистика,
хранение в PostgreSQL и импорт задач из внешнего REST API.

## Состав решения

| Проект | Назначение |
| --- | --- |
| `ConsoleApp.Core` | Библиотека: домен, EF Core, репозиторий, сервисы, HTTP-клиент |
| `ConsoleApp` | Консольное приложение (точка входа) с интерактивным меню и DI-хостом |
| `ConsoleApp.Tests` | Модульные и интеграционные тесты (xUnit + Testcontainers) |

## Быстрый старт

```powershell
# 1. Поднять PostgreSQL (порт 5433, чтобы не конфликтовать с локальным сервером)
docker compose up -d

# 2. Собрать и запустить
dotnet build
dotnet run --project ConsoleApp

# 3. Прогнать тесты (требуется запущенный Docker — Testcontainers)
dotnet test
```

Строка подключения задаётся в `ConsoleApp/appsettings.json` и переопределяется переменной
окружения `TODO_ConnectionStrings__TodoDb`.

## Чем можно проверить критерии чек-листа

| # | Критерий | Где смотреть / что делать |
| --- | --- | --- |
| 1 | Сквозной поиск | Поиск по всему решению идентификаторов `SearchAsync`, `ILike`, `GetStatisticsAsync` — встречаются в `Core`, `ConsoleApp`, `Tests` |
| 2 | Навигация по файлам | 3 проекта и папки `Domain`, `Data`, `Repositories`, `Services`, `Api`, `Commands`; переход к определению `ITaskRepository` → `EfTaskRepository` |
| 3 | Общие настройки IDE | `.editorconfig`, `Directory.Build.props`, `Directory.Packages.props`, `global.json` |
| 4 | Версионность языка | `Core/Services/LanguageFeatures.cs` и повсеместно: `record`, primary ctor, `required`, collection expressions, raw strings, pattern matching |
| 5 | Фреймворки и SDK | `global.json` (SDK 10.0.203), `TargetFramework=net10.0`, `Microsoft.Extensions.Hosting`, EF Core, Npgsql |
| 6 | Подсветка синтаксиса | Любой файл `*.cs` (атрибуты, интерполяция, raw strings в `LanguageFeatures.cs`) |
| 7 | IntelliSense | XML-документация в `Core`; перегрузки и дженерики в `EfTaskRepository`, extension-методы в `Extensions/TodoTaskExtensions.cs` |
| 8 | Несколько файлов | Откройте вкладки: `Program.cs`, `TodoService.cs`, `TodoDbContext.cs`, `TodoTask.cs` |
| 9 | Структура класса | `Core/Services/TodoService.cs`, `Core/Data/TodoDbContext.cs`, `Core/Domain/TodoTask.cs` |
| 10 | Анализаторы кода | `EnableNETAnalyzers`, `AnalysisLevel=latest-recommended`, `StyleCop.Analyzers`, настройки в `.globalconfig`, `GlobalSuppressions.cs` |
| 11 | Автоформатирование | `.editorconfig` + `dotnet format`; намеренно проверьте «Format Code» на `ConsoleOutput.cs` |
| 12 | Автогенерация кода | `Services/Dto/TodoJsonContext.cs`, `Api/ApiTodoJsonContext.cs` (source generation), `[GeneratedRegex]` в `TodoService.cs`, реализация `ITaskRepository` |
| 13 | Отладчик | `TodoService.GetStatisticsAsync`, `SeedDemoDataCommand` (цикл + переменные), `[DebuggerDisplay]` на `TodoTask`; исключения в `ImportFromApiCommand` |
| 14 | Git/SVN | Уже проверено отдельно |
| 15 | Сравнение файлов/версий | Сравните `samples/ConfigV1.json` и `samples/ConfigV2.json` |
| 16 | Управление зависимостями | `Directory.Packages.props` (центральные версии), граф проектов в `ConsoleApp.sln` |
| 17 | Инструменты БД | PostgreSQL в Docker, EF Core, миграции в `Core/Data/Migrations`, LINQ-запросы в `EfTaskRepository` |
| 18 | Инструменты тестирования | `ConsoleApp.Tests`; `dotnet test`; Test Explorer; `PostgresFixture` (Testcontainers) |
| 19 | Документация | XML-комментарии (`GenerateDocumentationFile`), этот README, `docs/architecture.md` |
| 20 | Инструменты API | `ConsoleApp/ConsoleApp.http` (REST-клиент), `ConsoleApp/openapi.json`, `Core/Api/` (`TodoApiClient`, `ApiPlaygroundClient`), пункт меню 13 «Проверка API (GET/POST)» |

## Меню приложения

```
 1. Список задач (с фильтрами)
 2. Добавить задачу
 3. Поиск задач по тексту
 4. Отметить задачу выполненной
 5. Удалить задачу
 6. Статистика по задачам
 7. Категории и теги
 8. Импорт задач из внешнего API
 9. Экспорт задач в JSON
10. Импорт задач из JSON
11. Загрузить демонстрационные данные
12. О приложении и среде выполнения
13. Проверка API (GET/POST)
 0. Выход
```

При первом запуске примените миграции (`dotnet ef database update`) или просто запустите
приложение — оно само выполнит `Database.Migrate()` и создаст схему.

## Полезные команды

```powershell
# Миграции EF Core
dotnet ef migrations add <Имя> --project ConsoleApp.Core --startup-project ConsoleApp.Core
dotnet ef database update --project ConsoleApp.Core --startup-project ConsoleApp.Core

# Проверка форматирования без изменений файлов
dotnet format --verify-no-changes

# Остановить базу
docker compose down
```

## Технологии

.NET 10 · C# 12–14 · Entity Framework Core 10 · Npgsql · PostgreSQL 18 · Docker ·
Microsoft.Extensions.Hosting · xUnit · Testcontainers · StyleCop.Analyzers
