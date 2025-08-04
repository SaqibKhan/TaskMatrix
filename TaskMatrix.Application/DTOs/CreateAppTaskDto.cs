using TaskMatrix.Domain.Enums;

namespace TaskMatrix.Application.DTOs;

public record CreateAppTaskDto(string Title, string? Description, TaskPriority Priority, DateTime DueDate, AppTaskStatus Status);