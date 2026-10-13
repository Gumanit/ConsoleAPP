using ConsoleApp.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsoleApp.Core.Data.Configurations;

/// <summary>
/// Конфигурация отображения <see cref="TodoTask"/> на таблицу базы данных.
/// </summary>
public sealed class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TodoTask> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("tasks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(4000);

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Tags)
            .WithMany(g => g.Tasks)
            .UsingEntity(join => join.ToTable("task_tags"));

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.DueDate);
    }
}
