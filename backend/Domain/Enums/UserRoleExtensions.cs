namespace Domain.Enums;

public static class UserRoleExtensions
{
    public static string ToStorageValue(this UserRole role) =>
        role switch
        {
            UserRole.Employer => "employer",
            UserRole.TeamLead => "teamlead",
            UserRole.Worker => "worker",
            _ => "worker"
        };
}
