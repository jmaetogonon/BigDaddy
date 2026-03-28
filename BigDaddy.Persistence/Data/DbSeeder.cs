using BigDaddy.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BigDaddy.Persistence.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedRolesAsync(db);
        await SeedPermissionsAsync(db);
        await SeedAdminUserAsync(db);
    }

    private static async Task SeedRolesAsync(AppDbContext db)
    {
        if (await db.Roles.AnyAsync()) return;

        db.Roles.AddRange(
            new Role { Name = "SystemAdministrator", Description = "Full system access" },
            new Role { Name = "Manager", Description = "Manages teams and users" },
            new Role { Name = "EndUser", Description = "Standard application user" }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(AppDbContext db)
    {
        var permissions = new[]
        {
            // User module
            ("Users", "UserList",   "View",          "users.list.view"),
            ("Users", "UserDetail", "View",          "users.detail.view"),
            ("Users", "UserList",   "Create",        "users.create"),
            ("Users", "UserList",   "Edit",          "users.edit"),
            ("Users", "UserList",   "Delete",        "users.delete"),
            ("Users", "UserList",   "Activate",      "users.activate"),
            ("Users", "UserList",   "Deactivate",    "users.deactivate"),
            ("Users", "UserList",   "Lock",          "users.lock"),
            ("Users", "UserList",   "Unlock",        "users.unlock"),
            ("Users", "UserList",   "ResetPassword", "users.reset-password"),
            ("Users", "UserList",   "AssignRoles",   "users.assign-roles"),
            ("Users", "UserList",   "AssignTeams",   "users.assign-teams"),
         };

        foreach (var (module, screen, action, code) in permissions)
        {
            if (!await db.Permissions.AnyAsync(p => p.Code == code))
                db.Permissions.Add(new Permission
                {
                    Module = module,
                    Screen = screen,
                    Action = action,
                    Code = code
                });
        }

        await db.SaveChangesAsync();

        // Assign all permissions to SystemAdministrator
        var adminRole = await db.Roles.FirstAsync(r => r.Name == "SystemAdministrator");
        var allPerms = await db.Permissions.ToListAsync();

        foreach (var perm in allPerms)
        {
            if (!await db.RolePermissions.AnyAsync(rp =>
                    rp.RoleId == adminRole.Id && rp.PermissionId == perm.Id))
            {
                db.RolePermissions.Add(new RolePermission
                { RoleId = adminRole.Id, PermissionId = perm.Id });
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync(u => u.Email == "admin@app.com")) return;

        var adminUser = new User
        {
            FirstName = "System",
            LastName = "Administrator",
            Username = "sysadmin",
            Email = "admin@app.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@12345"),
            IsActive = true,
            IsLocked = false
        };

        db.Users.Add(adminUser);
        await db.SaveChangesAsync();

        var adminRole = await db.Roles.FirstAsync(r => r.Name == "SystemAdministrator");
        db.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
        await db.SaveChangesAsync();
    }
}