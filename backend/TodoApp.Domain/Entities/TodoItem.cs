using TodoApp.Domain.Common;

namespace TodoApp.Domain.Entities;

public class TodoItem : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? Deadline { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public bool IsOverdue()
    {
        return Deadline.HasValue && Deadline.Value < DateTime.UtcNow && !IsCompleted;
    }
}
