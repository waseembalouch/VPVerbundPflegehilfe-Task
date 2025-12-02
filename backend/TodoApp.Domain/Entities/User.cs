using TodoApp.Domain.Common;

namespace TodoApp.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}
