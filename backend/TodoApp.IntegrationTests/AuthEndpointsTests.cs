using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TodoApp.Application.Commands.Auth.Register;
using TodoApp.Application.DTOs;
using Xunit;

namespace TodoApp.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<TodoApiFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthEndpointsTests(TodoApiFactory factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnCreated()
    {
        var request = new
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>(_jsonOptions);
        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithShortPassword_ShouldReturnBadRequest()
    {
        var request = new
        {
            Username = "testuser2",
            Email = "test2@example.com",
            Password = "Pass1"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ShouldReturnBadRequest()
    {
        var firstRequest = new
        {
            Username = "duplicateuser",
            Email = "first@example.com",
            Password = "Password123"
        };

        var secondRequest = new
        {
            Username = "duplicateuser",
            Email = "second@example.com",
            Password = "Password123"
        };

        await _client.PostAsJsonAsync("/api/auth/register", firstRequest);
        var response = await _client.PostAsJsonAsync("/api/auth/register", secondRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOk()
    {
        var registerRequest = new
        {
            Username = "logintest",
            Email = "logintest@example.com",
            Password = "Password123"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            Username = "logintest",
            Password = "Password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>(_jsonOptions);
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnBadRequest()
    {
        var loginRequest = new
        {
            Username = "nonexistent",
            Password = "WrongPassword123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
