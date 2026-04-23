using CapstoneProjectRegistration.API.Security;
using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CapstoneProjectRegistration.API.Infrastructure;

public static class IdentityDataSeeder
{
    public const string DevPassword = "DevPass@1";

    public static async Task SeedAsync(IServiceProvider services, ILogger logger, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CapstoneDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await context.Database.MigrateAsync(cancellationToken);

        foreach (var roleName in new[] { AppRoles.Admin, AppRoles.Lecturer, AppRoles.Student })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var r = new IdentityRole<int> { Name = roleName, NormalizedName = roleName.ToUpperInvariant() };
                var created = await roleManager.CreateAsync(r);
                if (!created.Succeeded)
                {
                    logger.LogError("Failed to create role {Role}: {Errors}", roleName,
                        string.Join("; ", created.Errors.Select(e => e.Description)));
                }
            }
        }

        await EnsureUserLinkedAsync(
            userManager,
            context,
            "admin@gmail.com",
            AppRoles.Admin,
            cancellationToken,
            linkAdminId: 1);

        await EnsureUserLinkedAsync(
            userManager,
            context,
            "lecturer1@gmail.com",
            AppRoles.Lecturer,
            cancellationToken,
            linkLecturerId: 1);

        await EnsureUserLinkedAsync(
            userManager,
            context,
            "thinhpdqse171589@fpt.edu.vn",
            AppRoles.Student,
            cancellationToken,
            linkStudentId: 1);
    }

    private static async Task EnsureUserLinkedAsync(
        UserManager<ApplicationUser> userManager,
        CapstoneDbContext context,
        string email,
        string role,
        CancellationToken cancellationToken,
        int? linkAdminId = null,
        int? linkLecturerId = null,
        int? linkStudentId = null)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var create = await userManager.CreateAsync(user, DevPassword);
            if (!create.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Cannot seed user {email}: {string.Join("; ", create.Errors.Select(e => e.Description))}");
            }

            await userManager.AddToRoleAsync(user, role);
        }
        else
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }

        if (linkAdminId is { } aid)
        {
            var row = await context.Admins.FirstAsync(a => a.Id == aid, cancellationToken);
            row.ApplicationUserId = user.Id;
        }

        if (linkLecturerId is { } lid)
        {
            var row = await context.Lecturers.FirstAsync(l => l.Id == lid, cancellationToken);
            row.ApplicationUserId = user.Id;
        }

        if (linkStudentId is { } sid)
        {
            var row = await context.Students.FirstAsync(s => s.Id == sid, cancellationToken);
            row.ApplicationUserId = user.Id;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
