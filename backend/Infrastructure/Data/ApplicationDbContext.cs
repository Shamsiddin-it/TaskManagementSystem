using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskEntity = Domain.Models.Task;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
