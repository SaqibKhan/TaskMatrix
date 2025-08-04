using TaskMatrix.Domain.Enums;

namespace TaskMatrix.Application.DTOs
{
    public record AppTaskDto(int Id, string title, string? Description, TaskPriority Priority, DateTime DueDate, AppTaskStatus Status);
}
