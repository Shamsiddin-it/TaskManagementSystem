using System;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Seeds;

public class DefaultRoles
{
    public static async System.Threading.Tasks.Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Employer", "Worker", "Team Lead" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}