using TaskMatrix.Domain.Enums;

namespace TaskMatrix.Application.DTOs;

public record UpdateAppTaskDto(int Id, string Title, string? Description, TaskPriority Priority, DateTime DueDate, AppTaskStatus Status);
