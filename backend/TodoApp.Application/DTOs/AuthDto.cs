namespace TodoApp.Application.DTOs;

public record AuthResponseDto(
    int Id,
    string Username,
    string Email,
    string Token);

public record UserDto(
    int Id,
    string Username,
    string Email,
    DateTime CreatedAt);
