using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskEntity = Domain.Models.Task;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<RetroActionItem> RetroActionItems => Set<RetroActionItem>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<SprintRetro> SprintRetros => Set<SprintRetro>();
    public DbSet<Subtask> Subtasks => Set<Subtask>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Absence> Absences => Set<Absence>();
    // public DbSet<User> Users => Set<User>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
    public DbSet<TaskTimeLog> TaskTimeLogs => Set<TaskTimeLog>();
    public DbSet<Project> Projects { get; set; }
    // public DbSet<Team> Teams { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<ProjectRisk> ProjectRisks { get; set; }
    public DbSet<ProjectTimeline> ProjectTimelines { get; set; }
    public DbSet<ProjectComment> ProjectComments { get; set; }
    public DbSet<ProjectChecklist> ProjectChecklists { get; set; }
    public DbSet<OrgBudget> OrgBudgets { get; set; }
    public DbSet<BudgetRecord> BudgetRecords { get; set; }
    public DbSet<EmployerNotification> EmployerNotifications { get; set; }
    public DbSet<WorkspaceSettings> WorkspaceSettings { get; set; }
    public DbSet<WorkspaceIntegration> WorkspaceIntegrations { get; set; }
    public DbSet<MemberSkill> MemberSkills { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<JoinRequest> JoinRequests => Set<JoinRequest>();
    public DbSet<Absence> Absences => Set<Absence>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<DaySummary> DaySummaries => Set<DaySummary>();
    public DbSet<ScheduleEvent> ScheduleEvents => Set<ScheduleEvent>();
    public DbSet<FocusSession> FocusSessions => Set<FocusSession>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<UserPresence> UserPresences => Set<UserPresence>();
    public DbSet<TaskTemplate> TaskTemplates => Set<TaskTemplate>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<TimeLog> TimeLogs => Set<TimeLog>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    // public DbSet<TeamMember> TeamMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectRiskConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectTimelineConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectCommentConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectChecklistConfiguration());
        modelBuilder.ApplyConfiguration(new OrgBudgetConfiguration());
        modelBuilder.ApplyConfiguration(new BudgetRecordConfiguration());
        modelBuilder.ApplyConfiguration(new EmployerNotificationConfiguration());
        modelBuilder.ApplyConfiguration(new WorkspaceSettingsConfiguration());
        modelBuilder.ApplyConfiguration(new WorkspaceIntegrationConfiguration());
        modelBuilder.ApplyConfiguration(new MemberSkillConfiguration());
        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        // // User
        // modelBuilder.Entity<User>(entity =>
        // {
        //     entity.HasKey(e => e.Id);
        //     entity.HasIndex(e => e.Email).IsUnique();
        // });

        // Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Employer)
                .WithMany()
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Team
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TeamMember
        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Team)
                .WithMany()
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.TeamId, e.UserId }).IsUnique();
        });

        // Task (full name avoids clash with System.Threading.Tasks.Task)
        modelBuilder.Entity<Domain.Models.Task>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Tasks");
            entity.HasOne<Team>()
                .WithMany()
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Invitation
        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Team)
                .WithMany()
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(e => e.InvitedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // JoinRequest
        modelBuilder.Entity<JoinRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Team)
                .WithMany()
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TaskComment
        // modelBuilder.Entity<TaskComment>(entity =>
        // {
        //     entity.HasKey(e => e.Id);
        //     entity.HasOne(e => e.Task)
        //         .WithMany()
        //         .HasForeignKey(e => e.TaskId)
        //         .OnDelete(DeleteBehavior.Cascade);
        //     entity.HasOne(e => e.Author)
        //         .WithMany()
        //         .HasForeignKey(e => e.AuthorId)
        //         .OnDelete(DeleteBehavior.Restrict);
        // });

        // Absence
        modelBuilder.Entity<Absence>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Subtask
        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Task)
                .WithMany()
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TimeLog
        modelBuilder.Entity<TimeLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Task)
                .WithMany()
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Attachment
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Task)
                .WithMany()
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.UploadedBy)
                .WithMany()
                .HasForeignKey(e => e.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ActivityLog
        // modelBuilder.Entity<ActivityLog>(entity =>
        // {
        //     entity.HasKey(e => e.Id);
        //     entity.HasOne(e => e.Actor)
        //         .WithMany()
        //         .HasForeignKey(e => e.ActorId)
        //         .OnDelete(DeleteBehavior.Cascade);
        //     entity.HasOne(e => e.Task)
        //         .WithMany()
        //         .HasForeignKey(e => e.TaskId)
        //         .OnDelete(DeleteBehavior.SetNull);
        // });

        // UserSettings
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        // DaySummary
        modelBuilder.Entity<DaySummary>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ScheduleEvent
        modelBuilder.Entity<ScheduleEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Task)
                .WithMany()
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // FocusSession
        modelBuilder.Entity<FocusSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Domain.Models.Task>()
                .WithMany()
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Note
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserPresence
        modelBuilder.Entity<UserPresence>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        // Badge - no FKs

        // UserBadge
        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Badge)
                .WithMany()
                .HasForeignKey(e => e.BadgeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TaskTag
        modelBuilder.Entity<TaskTag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Task)
                .WithMany()
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Sprint
        modelBuilder.Entity<Sprint>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Team)
                .WithMany()
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                var updatedAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
                if (updatedAtProperty != null)
                {
                    updatedAtProperty.CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
