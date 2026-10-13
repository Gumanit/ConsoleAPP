using System.Diagnostics;
using ConsoleApp.Commands;
using ConsoleApp.Core.Api;
using ConsoleApp.Core.Data;
using ConsoleApp.Core.Repositories;
using ConsoleApp.Core.Services;
using ConsoleApp.Menu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddEnvironmentVariables(prefix: "TODO_");
builder.Logging.SetMinimumLevel(LogLevel.Warning);

var connectionString = builder.Configuration.GetConnectionString("TodoDb")
    ?? DesignTimeDbContextFactory.DefaultConnectionString;

builder.Services.AddDbContext<TodoDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();
builder.Services.AddScoped<TodoService>();

builder.Services.AddHttpClient<ITodoApiClient, TodoApiClient>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<ApiPlaygroundClient>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<ICommand, ListTasksCommand>();
builder.Services.AddScoped<ICommand, AddTaskCommand>();
builder.Services.AddScoped<ICommand, SearchTasksCommand>();
builder.Services.AddScoped<ICommand, CompleteTaskCommand>();
builder.Services.AddScoped<ICommand, DeleteTaskCommand>();
builder.Services.AddScoped<ICommand, ShowStatisticsCommand>();
builder.Services.AddScoped<ICommand, ListCategoriesCommand>();
builder.Services.AddScoped<ICommand, ImportFromApiCommand>();
builder.Services.AddScoped<ICommand, ExportJsonCommand>();
builder.Services.AddScoped<ICommand, ImportJsonCommand>();
builder.Services.AddScoped<ICommand, SeedDemoDataCommand>();
builder.Services.AddScoped<ICommand, AboutCommand>();
builder.Services.AddScoped<ICommand, ApiCheckCommand>();

builder.Services.AddSingleton<ConsoleMenu>();

using var host = builder.Build();

await EnsureDatabaseAsync(host);

var menu = host.Services.GetRequiredService<ConsoleMenu>();
await menu.RunAsync();

static async Task EnsureDatabaseAsync(IHost host)
{
    using var scope = host.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

    try
    {
        await db.Database.MigrateAsync();
        var connection = db.Database.GetDbConnection();
        Console.WriteLine($"[БД] Подключено к {connection.DataSource}/{connection.Database}.");
    }
    catch (Exception exception)
    {
        Console.WriteLine($"[БД] Не удалось подключиться к PostgreSQL: {exception.Message}");
        Console.WriteLine("[БД] Запустите базу командой: docker compose up -d");
    }
}

