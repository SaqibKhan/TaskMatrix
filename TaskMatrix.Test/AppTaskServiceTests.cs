using Moq;
using TaskMatrix.Application.DTOs;
using TaskMatrix.Application.Services;
using TaskMatrix.Domain.Entities;
using TaskMatrix.Domain.Enums;
using TaskMatrix.Domain.Interfaces;

namespace TaskMatrix.Test
{
    public class AppTaskServiceTests
    {
        private readonly Mock<IAppTaskRepository> _repoMock;
        private readonly AppTaskService _service;

        public AppTaskServiceTests()
        {
            _repoMock = new Mock<IAppTaskRepository>();
            _service = new AppTaskService(_repoMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto()
        {
            var appTask = new AppTask(1, "Title", "Desc", TaskPriority.High, DateTime.Today, AppTaskStatus.Pending);
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appTask);

            var result = await _service.GetByIdAsync(1);

            Assert.Equal(appTask.Id, result.Id);
            Assert.Equal(appTask.Title, result.Title);
            Assert.Equal(appTask.Description, result.Description);
            Assert.Equal(appTask.Priority, result.Priority);
            Assert.Equal(appTask.DueDate, result.DueDate);
            Assert.Equal(appTask.Status, result.Status);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsDtos()
        {
            //Arrange
            var appTasks = new List<AppTask>
            {
                new AppTask(1, "T1", "D1", TaskPriority.High, DateTime.Today, AppTaskStatus.Pending),
                new AppTask(2, "T2", "D2", TaskPriority.Low, DateTime.Today, AppTaskStatus.Completed)
            };

            //Act
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(appTasks);
            var result = await _service.GetAllAsync();

            //Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreateAsync_CreatesAndReturnsDto()
        {
            // Arrange
            var dto = new CreateAppTaskDto("Title", "Desc", TaskPriority.High, DateTime.Today, AppTaskStatus.Pending);
            var created = new AppTask(1, dto.Title, dto.Description, dto.Priority, dto.DueDate, dto.Status);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<AppTask>())).ReturnsAsync(created);
            // Act
            var result = await _service.CreateAsync(dto);
            // Assert
            Assert.Equal(created.Id, result.Id);
            Assert.Equal(created.Title, result.Title);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTask()
        {
            // Arrange
            var id = 1;
            var appTask = new AppTask(id, "Title", "Desc", TaskPriority.High, DateTime.Today, AppTaskStatus.Pending);
            var dto = new UpdateAppTaskDto(id, "NewTitle", "NewDesc", TaskPriority.Low, DateTime.Today, AppTaskStatus.Completed);
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(appTask);
            _repoMock.Setup(r => r.UpdateAsync(appTask)).Returns(Task.CompletedTask);
            // Act
            await _service.UpdateAsync(dto);
            // Assert
            _repoMock.Verify(r => r.UpdateAsync(appTask), Times.Once);
            Assert.Equal("NewTitle", appTask.Title);
            Assert.Equal("NewDesc", appTask.Description);
            Assert.Equal(TaskPriority.Low, appTask.Priority);
            Assert.Equal(AppTaskStatus.Completed, appTask.Status);
        }



        [Fact]
        public async Task DeleteAsync_DeletesTask()
        {
            //arrange
            _repoMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
            // act
            await _service.DeleteAsync(1);
            // assert
            _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetPagedAsync_ReturnsPagedDtos()
        {
            // Arrange
            var appTasks = new List<AppTask>
            {
                new AppTask(1, "T1", "D1", TaskPriority.High, DateTime.Today, AppTaskStatus.Pending),
                new AppTask(2, "T2", "D2", TaskPriority.Low, DateTime.Today, AppTaskStatus.Completed)
            };
            _repoMock.Setup(r => r.GetPagedAsync(0, 2)).ReturnsAsync(appTasks);

            // Act
            var result = await _service.GetPagedAsync(0, 2);
            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}
