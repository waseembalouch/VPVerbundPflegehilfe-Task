using FluentAssertions;
using TodoApp.Domain.Entities;
using Xunit;

namespace TodoApp.UnitTests.Domain;

public class TodoItemTests
{
    [Fact]
    public void IsOverdue_WhenDeadlineIsInPastAndNotCompleted_ShouldReturnTrue()
    {
        var todoItem = new TodoItem
        {
            Description = "Test todo item that is overdue",
            Deadline = DateTime.UtcNow.AddDays(-1),
            IsCompleted = false
        };

        var result = todoItem.IsOverdue();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_WhenDeadlineIsInPastButCompleted_ShouldReturnFalse()
    {
        var todoItem = new TodoItem
        {
            Description = "Test todo item that is completed",
            Deadline = DateTime.UtcNow.AddDays(-1),
            IsCompleted = true
        };

        var result = todoItem.IsOverdue();

        result.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WhenDeadlineIsInFuture_ShouldReturnFalse()
    {
        var todoItem = new TodoItem
        {
            Description = "Test todo item with future deadline",
            Deadline = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        var result = todoItem.IsOverdue();

        result.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WhenDeadlineIsNull_ShouldReturnFalse()
    {
        var todoItem = new TodoItem
        {
            Description = "Test todo item without deadline",
            Deadline = null,
            IsCompleted = false
        };

        var result = todoItem.IsOverdue();

        result.Should().BeFalse();
    }

    [Fact]
    public void TodoItem_ShouldInitializeWithDefaultValues()
    {
        var todoItem = new TodoItem();

        todoItem.Description.Should().Be(string.Empty);
        todoItem.IsCompleted.Should().BeFalse();
        todoItem.Deadline.Should().BeNull();
    }
}
