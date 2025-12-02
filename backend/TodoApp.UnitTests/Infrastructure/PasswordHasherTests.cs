using FluentAssertions;
using TodoApp.Infrastructure.Authentication;
using Xunit;

namespace TodoApp.UnitTests.Infrastructure;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_ShouldReturnHashedPassword()
    {
        var password = "MySecurePassword123";

        var hashedPassword = _passwordHasher.HashPassword(password);

        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(password);
        hashedPassword.Should().StartWith("$2a$");
    }

    [Fact]
    public void HashPassword_ShouldReturnDifferentHashForSamePassword()
    {
        var password = "MySecurePassword123";

        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        var password = "MySecurePassword123";
        var hashedPassword = _passwordHasher.HashPassword(password);

        var result = _passwordHasher.VerifyPassword(password, hashedPassword);

        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        var password = "MySecurePassword123";
        var wrongPassword = "WrongPassword123";
        var hashedPassword = _passwordHasher.HashPassword(password);

        var result = _passwordHasher.VerifyPassword(wrongPassword, hashedPassword);

        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
    {
        var password = "MySecurePassword123";
        var hashedPassword = _passwordHasher.HashPassword(password);

        var result = _passwordHasher.VerifyPassword("", hashedPassword);

        result.Should().BeFalse();
    }
}
