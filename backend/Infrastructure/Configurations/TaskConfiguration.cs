using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskEntity = Domain.Models.Task;

namespace Infrastructure.Persistence.Configurations;

public sealed class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.TicketCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.TeamId)
            .IsRequired();

        builder.Property(x => x.AssignedToId)
            .IsRequired();

        builder.Property(x => x.CreatedById)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasDefaultValue(TaskStatus.Todo);

        builder.Property(x => x.Priority)
            .HasDefaultValue(TaskPriority.Medium);

        builder.Property(x => x.TicketType)
            .HasDefaultValue(TicketType.Task);

        builder.Property(x => x.OrderIndex)
            .HasDefaultValue(0);

        builder.Property(x => x.IsBlocked)
            .HasDefaultValue(false);

        builder.Property(x => x.TotalTimeMinutes)
            .HasDefaultValue(0);

        builder.Property(x => x.IsArchived)
            .HasDefaultValue(false);

        builder.HasIndex(x => new { x.TeamId, x.TicketCode })
            .IsUnique();

        builder.HasIndex(x => x.AssignedToId);
        builder.HasIndex(x => x.SprintId);

        builder.HasOne(x => x.Team)
            .WithMany()
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedTo)
            .WithMany()
            .HasForeignKey(x => x.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Sprint)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.SprintId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
