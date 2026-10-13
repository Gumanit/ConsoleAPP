using ConsoleApp.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsoleApp.Core.Data.Configurations;

/// <summary>
/// Конфигурация отображения <see cref="Category"/> на таблицу базы данных.
/// </summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("categories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Color)
            .HasMaxLength(9);

        builder.HasIndex(c => c.Name)
            .IsUnique();
    }
}
