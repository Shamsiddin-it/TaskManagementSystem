using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Team> Teams { get; set; }
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
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }

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
        modelBuilder.ApplyConfiguration(new TeamMemberConfiguration());
    }
}
