using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskMatrix.Application.DTOs;
using TaskMatrix.Application.Interfaces;
using TaskMatrix.Domain.Enums;
using TaskMatrix.WebAPI.Controllers;

namespace TaskMatrix.Test
{
    public class AppTaskControllerTests
    {
        private readonly Mock<IAppTaskService> _serviceMock;
        private readonly Mock<ILogger<AppTaskController>> _loggerMock;
        private readonly AppTaskController _controller;

        public AppTaskControllerTests()
        {
            _serviceMock = new Mock<IAppTaskService>();
            _loggerMock = new Mock<ILogger<AppTaskController>>();
            _controller = new AppTaskController(_loggerMock.Object, _serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResultWithTasks()
        {
            var tasks = new List<AppTaskDto>();
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(tasks);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(tasks, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResultWithTask()
        {
            var task = new AppTaskDto(
                Id: 1,
                Title: "Test Title",
                Description: "Test Description",
                Priority: TaskPriority.Medium, // Replace with a valid TaskPriority value
                DueDate: DateTime.UtcNow,
                Status: AppTaskStatus.Pending // Replace with a valid AppTaskStatus value
            );
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(task);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(task, okResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult()
        {
            var dto = new CreateAppTaskDto(
                Title: "Test Title",
                Description: "Test Description",
                Priority: TaskPriority.Medium,
                DueDate: DateTime.UtcNow,
                Status: AppTaskStatus.Pending
            );
            var created = new AppTaskDto(
                Id: 1,
                Title: "Test Title",
                Description: "Test Description",
                Priority: TaskPriority.Medium,
                DueDate: dto.DueDate,
                Status: AppTaskStatus.Pending
            );
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
            Assert.Equal("GetById", createdResult.ActionName);
        }

       

        [Fact]
        public async Task BatchCreate_ReturnsBadRequest_WhenNullOrEmpty()
        {
            var resultNull = await _controller.BatchCreate(null);
            var resultEmpty = await _controller.BatchCreate(new List<CreateAppTaskDto>());

            var badRequestNull = Assert.IsType<BadRequestObjectResult>(resultNull);
            var badRequestEmpty = Assert.IsType<BadRequestObjectResult>(resultEmpty);
            Assert.Equal("No items provided for update.", badRequestNull.Value);
            Assert.Equal("No items provided for update.", badRequestEmpty.Value);
        }

        [Fact]
        public async Task Update_ReturnsNoContent()
        {
            var dto = new UpdateAppTaskDto
            (
                Id: 1,
                Title: "Test Title",
                Description: "Test Description",
                Priority: TaskPriority.Medium,
                DueDate: DateTime.UtcNow,
                Status: AppTaskStatus.Pending
            );
            _serviceMock.Setup(s => s.UpdateAsync(dto)).Returns(Task.CompletedTask);

            var result = await _controller.Update(dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetPaged_ReturnsOkResultWithTasks()
        {
            var tasks = new List<AppTaskDto>
            {
                new AppTaskDto(
                    Id: 1,
                    Title: "Test Title",
                    Description: "Test Description",
                    Priority: TaskPriority.Medium,
                    DueDate: DateTime.UtcNow,
                    Status: AppTaskStatus.Pending
                )
            };
            _serviceMock.Setup(s => s.GetPagedAsync(0, 20)).ReturnsAsync(tasks);

            var result = await _controller.GetPaged(0, 20);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(tasks, okResult.Value);
        }
    }
}