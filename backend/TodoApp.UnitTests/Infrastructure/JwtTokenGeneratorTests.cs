using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Authentication;
using Xunit;

namespace TodoApp.UnitTests.Infrastructure;

public class JwtTokenGeneratorTests
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;

    public JwtTokenGeneratorTests()
    {
        var configurationValues = new Dictionary<string, string>
        {
            { "Jwt:Secret", "ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456789" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpirationHours", "24" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues!)
            .Build();

        _jwtTokenGenerator = new JwtTokenGenerator(_configuration);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        var token = _jwtTokenGenerator.GenerateToken(user);

        token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Should().NotBeNull();
        jwtToken.Issuer.Should().Be("TestIssuer");
        jwtToken.Audiences.Should().Contain("TestAudience");
    }

    [Fact]
    public void GenerateToken_ShouldIncludeUserClaims()
    {
        var user = new User
        {
            Id = 42,
            Username = "johndoe",
            Email = "john@example.com",
            PasswordHash = "hashedpassword"
        };

        var token = _jwtTokenGenerator.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        subClaim.Should().NotBeNull();
        subClaim!.Value.Should().Be("42");

        var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
        usernameClaim.Should().NotBeNull();
        usernameClaim!.Value.Should().Be("johndoe");

        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);
        emailClaim.Should().NotBeNull();
        emailClaim!.Value.Should().Be("john@example.com");

        var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
        jtiClaim.Should().NotBeNull();
    }

    [Fact]
    public void GenerateToken_ShouldSetExpirationTime()
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        var beforeGeneration = DateTime.UtcNow;

        var token = _jwtTokenGenerator.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.ValidTo.Should().BeAfter(beforeGeneration.AddHours(23));
        jwtToken.ValidTo.Should().BeBefore(beforeGeneration.AddHours(25));
    }

    [Fact]
    public void GenerateToken_ShouldGenerateDifferentTokensForSameUser()
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        var token1 = _jwtTokenGenerator.GenerateToken(user);
        var token2 = _jwtTokenGenerator.GenerateToken(user);

        token1.Should().NotBe(token2);
    }
}
