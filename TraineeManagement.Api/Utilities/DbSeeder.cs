using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
 
namespace TraineeManagement.Api.Utilities;
 
public static class DbSeeder
{
    public static async Task SeedAdminUserAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            return;
        }
 
        var passwordHasher = new PasswordHasher<User>();
 
        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };
 
        adminUser.PasswordHash = passwordHasher.HashPassword(
            adminUser,
            "admin123");
 
        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}