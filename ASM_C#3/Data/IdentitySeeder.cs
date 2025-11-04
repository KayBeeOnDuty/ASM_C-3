using ASM_C_3.Data;
using ASM_C_3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_C_3.Data
{
    public static class IdentitySeeder
    {
        private static readonly string[] Roles = new[] { "Admin", "Staff", "User" };

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var db = services.GetRequiredService<TraNgheDbContext>();

            // Ensure roles
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed default admin: username=admin, password=abc@123
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "abc@123");
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create default admin: {errors}");
                }
            }

            // Ensure admin is in Admin role only
            var roles = await userManager.GetRolesAsync(adminUser);
            if (!roles.Contains("Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            // Optional: remove other roles to keep a single role assignment for admin
            foreach (var r in roles.Where(r => r != "Admin"))
            {
                await userManager.RemoveFromRoleAsync(adminUser, r);
            }
        }
    }
}

