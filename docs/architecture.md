# Архитектура приложения

## Обзор

Приложение построено по слоям: домен → доступ к данным → прикладные сервисы → интерфейс (CLI).

```
ConsoleApp (exe)
  Program.cs            — хост, DI, конфигурация, применение миграций
  Menu/                 — ConsoleMenu, ConsolePrompt, ConsoleOutput
  Commands/             — по классу на пункт меню, реализуют ICommand
        │
        ▼
ConsoleApp.Core (classlib)
  Api/                  — TodoApiClient (HttpClient + source-gen JSON)
  Services/             — TodoService (бизнес-логика), DTO + JsonSerializerContext
  Repositories/         — ITaskRepository, EfTaskRepository, TaskFilter
  Data/                 — TodoDbContext, конфигурации сущностей, миграции
  Domain/               — TodoTask, Category, Tag, Priority, TodoStatus
  Extensions/           — extension-методы для задач
        │
        ▼
PostgreSQL 18 (Docker) / Testcontainers (тесты)
```

## Поток данных

1. `Program.cs` создаёт `IHost`, регистрирует `TodoDbContext` (Npgsql) и сервисы.
2. При старте выполняется `Database.MigrateAsync()`.
3. `ConsoleMenu` на каждую итерацию создаёт DI-область и разрешает все `ICommand`.
4. Команда вызывает `TodoService` (или `ITaskRepository`), тот — `TodoDbContext`.
5. Результат печатается через `ConsoleOutput` и extension-метод `Describe()`.

## Ключевые решения

- **Интерфейсы и DI** — `ITaskRepository`, `ITodoApiClient`, `ICommand` позволяют заменять
  реализации и упрощают тестирование.
- **EF Core (Npgsql)** — код-первый подход, конфигурации сущностей в отдельных файлах,
  миграции в `Data/Migrations`.
- **System.Text.Json source generation** — `TodoJsonContext` и `ApiTodoJsonContext`
  исключают рефлексию при сериализации.
- **Testcontainers** — интеграционные тесты поднимают одноразовый PostgreSQL 18 в Docker.
- **Анализаторы** — .NET Analyzers + StyleCop с настройкой через `.globalconfig`.

## Схема данных

| Таблица | Назначение |
| --- | --- |
| `tasks` | Задачи (приоритет и статус хранятся строками) |
| `categories` | Категории, связь один-ко-многим с задачами |
| `tags` | Теги |
| `task_tags` | Связь многие-ко-многим задач и тегов |
