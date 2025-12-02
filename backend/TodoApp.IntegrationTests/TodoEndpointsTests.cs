using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TodoApp.Application.DTOs;
using Xunit;

namespace TodoApp.IntegrationTests;

public class TodoEndpointsTests : IClassFixture<TodoApiFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public TodoEndpointsTests(TodoApiFactory factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetTodos_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/todos");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTodos_WithAuthentication_ShouldReturnOk()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/todos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var todos = await response.Content.ReadFromJsonAsync<List<TodoDto>>(_jsonOptions);
        todos.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTodo_WithValidData_ShouldReturnCreated()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Description = "Integration test todo item with valid description",
            Deadline = DateTime.UtcNow.AddDays(7)
        };

        var response = await _client.PostAsJsonAsync("/api/todos", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var todo = await response.Content.ReadFromJsonAsync<TodoDto>(_jsonOptions);
        todo.Should().NotBeNull();
        todo!.Description.Should().Be(request.Description);
        todo.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task CreateTodo_WithShortDescription_ShouldReturnBadRequest()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Description = "Short",
            Deadline = (DateTime?)null
        };

        var response = await _client.PostAsJsonAsync("/api/todos", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTodo_WithValidData_ShouldReturnNoContent()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new
        {
            Description = "Todo to be updated with valid description",
            Deadline = (DateTime?)null
        };

        var createResponse = await _client.PostAsJsonAsync("/api/todos", createRequest);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoDto>(_jsonOptions);

        var updateRequest = new
        {
            Description = "Updated todo item with valid description",
            IsCompleted = true,
            Deadline = (DateTime?)null
        };

        var response = await _client.PutAsJsonAsync($"/api/todos/{createdTodo!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTodo_WithValidId_ShouldReturnNoContent()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new
        {
            Description = "Todo to be deleted with valid description",
            Deadline = (DateTime?)null
        };

        var createResponse = await _client.PostAsJsonAsync("/api/todos", createRequest);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoDto>(_jsonOptions);

        var response = await _client.DeleteAsync($"/api/todos/{createdTodo!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetTodoById_WhenTodoDoesNotExist_ShouldReturnNotFound()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/todos/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var username = $"testuser_{Guid.NewGuid():N}";
        var registerRequest = new
        {
            Username = username,
            Email = $"{username}@example.com",
            Password = "Password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>(_jsonOptions);

        return authResponse!.Token;
    }
}
