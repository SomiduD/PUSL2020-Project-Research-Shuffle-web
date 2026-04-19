using Microsoft.AspNetCore.Identity;

namespace BlindMatch.Web.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Define the system roles
            string[] roleNames = { "ModuleLeader", "Supervisor", "Student" };

            // 2. Automatically create the roles if they are missing
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 3. Automatically grant God-Mode to your admin account!
            var adminEmail = "admin@blindmatch.com"; // Make sure this matches the account you created!
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null)
            {
                // If the user exists but doesn't have the badge, give it to them
                if (!await userManager.IsInRoleAsync(adminUser, "ModuleLeader"))
                {
                    await userManager.AddToRoleAsync(adminUser, "ModuleLeader");
                }
            }
        }
    }
}