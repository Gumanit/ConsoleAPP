using ConsoleApp.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsoleApp.Core.Data.Configurations;

/// <summary>
/// Конфигурация отображения <see cref="Tag"/> на таблицу базы данных.
/// </summary>
public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("tags");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(t => t.Name)
            .IsUnique();
    }
}
