using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Data;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Tests.Services;

public class LocalAuthenticationServiceTests
{
    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsSuccessAndSetsCurrentUser()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);

        var eventRaised = false;
        service.AuthenticationStateChanged += (_, isAuthenticated) =>
        {
            eventRaised = isAuthenticated;
        };

        // Act
        var result = await service.LoginAsync("test@example.com", "Password123!");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Login successful", result.Message);
        Assert.True(service.IsAuthenticated);
        Assert.NotNull(service.CurrentUser);
        Assert.Equal("test@example.com", service.CurrentUser.Email);
        Assert.Contains("User", service.CurrentUserRoles);
        Assert.True(eventRaised);
    }

    [Fact]
    public async Task LoginAsync_WhenEmailDoesNotExist_ReturnsFailure()
    {
        // Arrange
        var context = CreateContext();
        var service = new LocalAuthenticationService(context);

        // Act
        var result = await service.LoginAsync("missing@example.com", "Password123!");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password", result.Message);
        Assert.False(service.IsAuthenticated);
        Assert.Null(service.CurrentUser);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ReturnsFailure()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);

        // Act
        var result = await service.LoginAsync("test@example.com", "WrongPassword");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password", result.Message);
        Assert.False(service.IsAuthenticated);
        Assert.Null(service.CurrentUser);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailIsNew_ReturnsSuccessAndCreatesUser()
    {
        // Arrange
        var context = CreateContext();
        SeedDefaultRole(context);

        var service = new LocalAuthenticationService(context);

        // Act
        var result = await service.RegisterAsync(
            "New",
            "User",
            "newuser@example.com",
            "Password123!"
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Registration successful", result.Message);

        var createdUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email == "newuser@example.com");

        Assert.NotNull(createdUser);
        Assert.Equal("New", createdUser.FirstName);
        Assert.Equal("User", createdUser.LastName);
        Assert.True(createdUser.IsActive);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);

        // Act
        var result = await service.RegisterAsync(
            "Duplicate",
            "User",
            "test@example.com",
            "Password123!"
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User with this email already exists", result.Message);
    }

    [Fact]
    public async Task LogoutAsync_WhenUserIsLoggedIn_ClearsCurrentUser()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);
        await service.LoginAsync("test@example.com", "Password123!");

        var logoutEventRaised = false;
        service.AuthenticationStateChanged += (_, isAuthenticated) =>
        {
            if (!isAuthenticated)
                logoutEventRaised = true;
        };

        // Act
        await service.LogoutAsync();

        // Assert
        Assert.False(service.IsAuthenticated);
        Assert.Null(service.CurrentUser);
        Assert.Empty(service.CurrentUserRoles);
        Assert.True(logoutEventRaised);
    }

    [Fact]
    public async Task HasRole_WhenUserHasRole_ReturnsTrue()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);
        await service.LoginAsync("test@example.com", "Password123!");

        // Act
        var result = service.HasRole("User");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasRole_WhenUserDoesNotHaveRole_ReturnsFalse()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);
        await service.LoginAsync("test@example.com", "Password123!");

        // Act
        var result = service.HasRole("Admin");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasAnyRole_WhenUserHasOneMatchingRole_ReturnsTrue()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);
        await service.LoginAsync("test@example.com", "Password123!");

        // Act
        var result = service.HasAnyRole("Admin", "User");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasAllRoles_WhenUserDoesNotHaveAllRoles_ReturnsFalse()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);
        await service.LoginAsync("test@example.com", "Password123!");

        // Act
        var result = service.HasAllRoles("User", "Admin");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenNoUserIsLoggedIn_ReturnsFalse()
    {
        // Arrange
        var context = CreateContext();
        var service = new LocalAuthenticationService(context);

        // Act
        var result = await service.ChangePasswordAsync("OldPassword", "NewPassword123!");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenCurrentPasswordIsWrong_ReturnsFalse()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);
        await service.LoginAsync("test@example.com", "Password123!");

        // Act
        var result = await service.ChangePasswordAsync("WrongPassword", "NewPassword123!");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenCurrentPasswordIsCorrect_ReturnsTrue()
    {
        // Arrange
        var context = CreateContext();
        SeedUserWithRole(context, "test@example.com", "Password123!");

        var service = new LocalAuthenticationService(context);
        await service.LoginAsync("test@example.com", "Password123!");

        // Act
        var result = await service.ChangePasswordAsync("Password123!", "NewPassword123!");

        // Assert
        Assert.True(result);

        var updatedUser = await context.Users
            .FirstAsync(u => u.Email == "test@example.com");

        Assert.True(BCrypt.Net.BCrypt.Verify("NewPassword123!", updatedUser.PasswordHash));
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    private static void SeedDefaultRole(AppDbContext context)
    {
        var role = new Role
        {
            Id = 1,
            Name = "User",
            Description = "Standard user",
            IsDefault = true
        };

        context.Roles.Add(role);
        context.SaveChanges();
    }

    private static void SeedUserWithRole(AppDbContext context, string email, string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, salt);

        var role = new Role
        {
            Id = 1,
            Name = "User",
            Description = "Standard user",
            IsDefault = true
        };

        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = salt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var userRole = new UserRole
        {
            Id = 1,
            UserId = user.Id,
            RoleId = role.Id,
            IsActive = true
        };

        context.Roles.Add(role);
        context.Users.Add(user);
        context.UserRoles.Add(userRole);
        context.SaveChanges();
    }
}