using StarterApp.Tests.Fixtures;

namespace StarterApp.Tests.Fixtures;

public class DatabaseFixtureTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public DatabaseFixtureTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DatabaseFixture_WhenCreated_SeedsUsers()
    {
        // Arrange
        var context = _fixture.Context;

        // Act
        var users = context.Users.ToList();

        // Assert
        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public void DatabaseFixture_WhenCreated_SeedsRoles()
    {
        // Arrange
        var context = _fixture.Context;

        // Act
        var roles = context.Roles.ToList();

        // Assert
        Assert.NotNull(roles);
        Assert.NotEmpty(roles);
        Assert.Equal(2, roles.Count);
    }
}