using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;

namespace TaskBridge.DB.Configurations;

/// <summary>
/// Configures persistence for <see cref="TaskHistory"/>.
/// </summary>
public sealed class TaskHistoryConfiguration : IEntityTypeConfiguration<TaskHistory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TaskHistory> builder)
    {
        builder.ToTable("task_history");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.TaskId)
            .HasColumnName("task_id")
            .IsRequired();

        builder.Property(x => x.ChangedBy)
            .HasColumnName("changed_by")
            .IsRequired();

        builder.Property(x => x.ChangeType)
            .HasColumnName("change_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.OldValue)
            .HasColumnName("old_value")
            .HasMaxLength(5000);

        builder.Property(x => x.NewValue)
            .HasColumnName("new_value")
            .HasMaxLength(5000);

        builder.Property(x => x.ChangedAt)
            .HasColumnName("changed_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne<TaskItem>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ChangedBy)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasIndex(x => x.TaskId);
        builder.HasIndex(x => x.ChangedBy);
        builder.HasIndex(x => x.ChangedAt);

        builder.HasIndex(x => new { x.TaskId, x.ChangedAt });
    }
}
