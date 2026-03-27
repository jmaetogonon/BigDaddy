using BigDaddy.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace BigDaddy.Persistence.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // ── Roles ─────────────────────────────────────────────────────────────
        if (!db.Roles.Any())
        {
            var roles = new List<Role>
        {
            new() { Name = "SystemAdministrator", Description = "Full system access", CreatedAt = DateTime.Parse("2026/01/01 10:00"), UpdatedAt = DateTime.Parse("2026/01/01 10:00") },
            new() { Name = "Manager",             Description = "Manages teams and users", CreatedAt = DateTime.Parse("2026/01/01 10:00"), UpdatedAt = DateTime.Parse("2026/01/01 10:00") },
            new() { Name = "EndUser",             Description = "Standard application user", CreatedAt = DateTime.Parse("2026/01/01 10:00"), UpdatedAt = DateTime.Parse("2026/01/01 10:00") },
        };
            db.Roles.AddRange(roles);
        }

        // ── Permissions ───────────────────────────────────────────────────────
        if (!db.Permissions.Any())
        {
            var permissions = new List<Permission>
        {
            new() {  Module = "Users", Screen = "UserList", Action = "View",   Code = "users.userlist.view", CreatedAt = DateTime.Parse("2026/01/01 10:00"), UpdatedAt = DateTime.Parse("2026/01/01 10:00")   },
            new() {  Module = "Users", Screen = "UserList", Action = "Create", Code = "users.userlist.create", CreatedAt = DateTime.Parse("2026/01/01 10:00"), UpdatedAt = DateTime.Parse("2026/01/01 10:00") },
            new() {  Module = "Users", Screen = "UserList", Action = "Edit",   Code = "users.userlist.edit", CreatedAt = DateTime.Parse("2026/01/01 10:00"), UpdatedAt = DateTime.Parse("2026/01/01 10:00")   },
            new() {  Module = "Users", Screen = "UserList", Action = "Delete", Code = "users.userlist.delete", CreatedAt = DateTime.Parse("2026/01/01 10:00"), UpdatedAt = DateTime.Parse("2026/01/01 10:00") },
            new() {  Module = "Users", Screen = "UserForm", Action = "View",   Code = "users.userform.view", CreatedAt = DateTime.Parse("2026/01/01 10:00"), UpdatedAt = DateTime.Parse("2026/01/01 10:00")   },
            // add more as needed
        };
            db.Permissions.AddRange(permissions);
        }

        await db.SaveChangesAsync();

        // ── Assign all permissions to SystemAdministrator ─────────────────────
        var adminRole = db.Roles.First(r => r.Name == "SystemAdministrator");
        var allPerms = db.Permissions.ToList();

        foreach (var perm in allPerms)
        {
            if (!db.RolePermissions.Any(rp => rp.RoleId == adminRole.Id && rp.PermissionId == perm.Id))
                db.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = perm.Id });
        }

        await db.SaveChangesAsync();

        // ── Default Admin User ────────────────────────────────────────────────
        if (!db.Users.Any(u => u.Email == "admin@app.com"))
        {
            var adminUser = new User
            {
                FirstName = "System",
                LastName = "Administrator",
                Username = "sysadmin",
                Email = "admin@app.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@12345"),
                MobileNumber = null,
                IsActive = true,
                IsLocked = false,
                CreatedAt = DateTime.Parse("2026/01/01 10:00"),
                UpdatedAt = DateTime.Parse("2026/01/01 10:00")
            };

            db.Users.Add(adminUser);
            await db.SaveChangesAsync();

            // Assign SystemAdministrator role to admin user
            db.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
            await db.SaveChangesAsync();
        }
    }
}