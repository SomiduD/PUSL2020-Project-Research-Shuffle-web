using Microsoft.AspNetCore.Identity;

namespace BlindMatch.Web.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Create the Roles if they don't exist
            string[] roleNames = { "Supervisor", "Student" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Create a default test Supervisor
            if (await userManager.FindByEmailAsync("supervisor@test.com") == null)
            {
                var supervisor = new IdentityUser { UserName = "supervisor@test.com", Email = "supervisor@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(supervisor, "Password123!");
                await userManager.AddToRoleAsync(supervisor, "Supervisor");
            }

            // 3. Create a default test Student
            if (await userManager.FindByEmailAsync("student@test.com") == null)
            {
                var student = new IdentityUser { UserName = "student@test.com", Email = "student@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(student, "Password123!");
                await userManager.AddToRoleAsync(student, "Student");
            }
        }
    }
}