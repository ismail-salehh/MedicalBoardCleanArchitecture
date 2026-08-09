using MedicalBoard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Infrastructure.Data;

public static class DbInitializer
{
    // Call this once at startup (e.g. right after app.Build(), behind an env check) to
    // guarantee a login-capable account exists. Safe to call repeatedly — it's idempotent.
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Roles.AnyAsync(r => r.Name == "Administrator"))
        {
            db.Roles.Add(new Role { Name = "Administrator", Description = "Full system access" });
            await db.SaveChangesAsync();
        }

        if (!await db.Users.AnyAsync(u => u.Username == "admin"))
        {
            var adminRole = await db.Roles.FirstAsync(r => r.Name == "Administrator");
            var hasher = new PasswordHasher<User>();

            var admin = new User
            {
                Username = "admin",
                Email = "admin@medicalboard.local",
                IsActive = true
            };
            admin.PasswordHash = hasher.HashPassword(admin, "ChangeMe123!");

            db.Users.Add(admin);
            await db.SaveChangesAsync();

            db.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = adminRole.Id });
            await db.SaveChangesAsync();
        }
    }
}