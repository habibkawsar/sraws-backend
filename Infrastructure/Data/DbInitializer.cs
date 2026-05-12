using Microsoft.EntityFrameworkCore;
using SponsorshipApproval.Api.Application.Interfaces;
using SponsorshipApproval.Api.Domain.Entities;

namespace SponsorshipApproval.Api.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        await dbContext.Database.MigrateAsync();

        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var passwordHash = passwordHasher.Hash("Password123!");
        var users = new[]
        {
            new User { FullName = "Rina Requestor", Email = "requestor@test.com", Department = "Marketing", RoleId = 1, PasswordHash = passwordHash },
            new User { FullName = "Mahmud Manager", Email = "manager@test.com", Department = "Marketing", RoleId = 2, PasswordHash = passwordHash },
            new User { FullName = "Farzana Finance", Email = "finance@test.com", Department = "Finance", RoleId = 3, PasswordHash = passwordHash },
            new User { FullName = "Samira Admin", Email = "admin@test.com", Department = "IT Governance", RoleId = 4, PasswordHash = passwordHash }
        };

        dbContext.Users.AddRange(users);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Seeded demo users for Sponsorship Request Approval Workflow System.");
    }
}
