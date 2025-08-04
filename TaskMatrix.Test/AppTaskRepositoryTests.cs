using Microsoft.EntityFrameworkCore;
using TaskMatrix.Domain.Entities;
using TaskMatrix.Domain.Enums;
using TaskMatrix.Infrastructure.Data;
using TaskMatrix.Infrastructure.Repositories;

namespace TaskMatrix.Test
{
    public class AppTaskRepositoryTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        private AppTask CreateTask(int id = 1, string title = "Title", string desc = "Desc", TaskPriority priority = TaskPriority.High, AppTaskStatus status = AppTaskStatus.Pending)
        {
            return new AppTask(id, title, desc, priority, DateTime.Today, status);
        }

        [Fact]
        public async Task AddAsync_AddsTask()
        {
            // Arrange
            var context = GetDbContext(nameof(AddAsync_AddsTask));
            var repo = new AppTaskRepository(context);
            var task = CreateTask();

            // Act
            var result = await repo.AddAsync(task);

            // Assert
            Assert.Equal(task.Id, result.Id);
            Assert.Single(context.AppTasks);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTask()
        {
            // Arrange
            var context = GetDbContext(nameof(GetByIdAsync_ReturnsTask));
            var repo = new AppTaskRepository(context);
            var task = CreateTask();
            context.AppTasks.Add(task);
            context.SaveChanges();

            // Act
            var result = await repo.GetByIdAsync(task.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTasks()
        {
            // Arrange
            var context = GetDbContext(nameof(GetAllAsync_ReturnsAllTasks));
            var repo = new AppTaskRepository(context);
            context.AppTasks.Add(CreateTask(1));
            context.AppTasks.Add(CreateTask(2, "T2"));
            context.SaveChanges();

            // Act
            var result = await repo.GetAllAsync();

            //assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTask()
        {
            // Arrange
            var context = GetDbContext(nameof(UpdateAsync_UpdatesTask));
            var repo = new AppTaskRepository(context);
            var task = CreateTask();
            context.AppTasks.Add(task);
            context.SaveChanges();

            // Act
            task.UpdateAppTask("Updated", "UpdatedDesc", (int)TaskPriority.Low, AppTaskStatus.Completed);
            await repo.UpdateAsync(task);
 
            // Assert
            var updated = await repo.GetByIdAsync(task.Id);
            Assert.Equal("Updated", updated.Title);
            Assert.Equal(TaskPriority.Low, updated.Priority);
            Assert.Equal(AppTaskStatus.Completed, updated.Status);
        }

        [Fact]
        public async Task DeleteAsync_DeletesTask()
        {
            // Arrange
            var context = GetDbContext(nameof(DeleteAsync_DeletesTask));
            var repo = new AppTaskRepository(context);
            var task = CreateTask();
            context.AppTasks.Add(task);
            context.SaveChanges();

            // Act
            await repo.DeleteAsync(task.Id);

            //assert
            Assert.Empty(context.AppTasks);
        }

        [Fact]
        public async Task GetPagedAsync_ReturnsPagedTasks()
        {
            // Arrange
            var context = GetDbContext(nameof(GetPagedAsync_ReturnsPagedTasks));
            var repo = new AppTaskRepository(context);
            context.AppTasks.Add(CreateTask(1));
            context.AppTasks.Add(CreateTask(2, "T2"));
            context.AppTasks.Add(CreateTask(3, "T3"));
            context.SaveChanges();
            // Act
            var result = await repo.GetPagedAsync(1, 2);
            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Title == "T2");
            Assert.Contains(result, t => t.Title == "T3");
        }

        [Fact]
        public async Task DeleteAsync_NonExistentTask_DoesNothing()
        {
            // Arrange
            var context = GetDbContext(nameof(DeleteAsync_NonExistentTask_DoesNothing));
            var repo = new AppTaskRepository(context);
            context.AppTasks.Add(CreateTask(2, "T2"));
            context.SaveChanges();
            // Act
            await repo.DeleteAsync(2);
            // Assert
            Assert.Empty(context.AppTasks);
        }
    }
}