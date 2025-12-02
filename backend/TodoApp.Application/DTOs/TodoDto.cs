namespace TodoApp.Application.DTOs;

public record TodoDto
{
    public int Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public DateTime? Deadline { get; init; }
    public bool IsOverdue { get; init; }
    public DateTime CreatedAt { get; init; }
}
