using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
 
namespace TraineeManagement.Api.Utilities;
 
public static class DbSeeder
{
    public static async Task SeedAdminUserAsync(AppDbContext context)
    {
        var username = Environment.GetEnvironmentVariable("DbSeeder_Username");
        var password=Environment.GetEnvironmentVariable("DbSeeder_Password");
        var email = Environment.GetEnvironmentVariable("DbSeeder_Email");

        if (await context.Users.AnyAsync(u => u.Username == username))
        {
            return;
        }
 
        var passwordHasher = new PasswordHasher<User>();
 
        var adminUser = new User
        {
            Username = username,
            Email = email,
            Role = UserRole.Admin,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };
 
        adminUser.PasswordHash = passwordHasher.HashPassword(
            adminUser,
            password
        );
 
        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}