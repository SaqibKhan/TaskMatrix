using System.Diagnostics;
using System.Xml.Linq;
using TaskMatrix.Domain.Enums;

namespace TaskMatrix.Domain.Entities;

public class AppTask
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public AppTaskStatus Status { get; set; }

    public AppTask(int id, string title, string? description, TaskPriority priority, DateTime dueDate, AppTaskStatus status)
    {
        Id = id;
        Title = title ?? throw new ArgumentNullException(@"Title cannot be empty");
        Description = description;
        Priority = (priority < 0) ? throw new ArgumentNullException($"Priority must be greater than zero") : priority;
        DueDate = dueDate;
        Status = status;
    }

    public void UpdateAppTask(string title, string? description, int priority, AppTaskStatus status)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentNullException(@"Title cannot be empty");
        if (priority <= 0)
            throw new ArgumentNullException($"Priority must be greater than zero");

        title = title;
        Description = description ?? "";
        priority = priority;
        status = status;
    }

  
}
