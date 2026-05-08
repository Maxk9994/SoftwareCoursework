using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Data;
using StarterApp.Database.Models;

namespace StarterApp.Tests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public AppDbContext Context { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new AppDbContext(options);

        Context.Database.EnsureCreated();

        SeedTestData();
    }

    private void SeedTestData()
    {
        var roles = new List<Role>
        {
            new Role
            {
                Id = 1,
                Name = "User",
                Description = "Standard application user"
            },
            new Role
            {
                Id = 2,
                Name = "Admin",
                Description = "Administrator user"
            }
        };

        Context.Roles.AddRange(roles);

        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Email = "owner@example.com",
                FirstName = "Owner",
                LastName = "User",
                PasswordHash = "test-hash",
                PasswordSalt = "test-salt"
            },
            new User
            {
                Id = 2,
                Email = "renter@example.com",
                FirstName = "Renter",
                LastName = "User",
                PasswordHash = "test-hash",
                PasswordSalt = "test-salt"
            }
        };

        Context.Users.AddRange(users);

        var userRoles = new List<UserRole>
        {
            new UserRole
            {
                Id = 1,
                UserId = 1,
                RoleId = 1
            },
            new UserRole
            {
                Id = 2,
                UserId = 2,
                RoleId = 1
            }
        };

        Context.UserRoles.AddRange(userRoles);

        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}