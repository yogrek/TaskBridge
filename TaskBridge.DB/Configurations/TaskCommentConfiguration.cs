using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;

namespace TaskBridge.DB.Configurations;

/// <summary>
/// Configures persistence for <see cref="TaskComment"/>.
/// </summary>
public sealed class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        builder.ToTable("task_comments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.TaskId)
            .HasColumnName("task_id")
            .IsRequired();

        builder.Property(x => x.AuthorId)
            .HasColumnName("author_id")
            .IsRequired();

        builder.Property(x => x.Text)
            .HasColumnName("text")
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne<TaskItem>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AuthorId);
        builder.HasIndex(x => x.TaskId);
        builder.HasIndex(x => x.CreatedAt);
    }
}
