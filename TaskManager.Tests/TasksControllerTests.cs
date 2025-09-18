using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Api.Data;
using TaskManager.Api.Data.Entities;
using TaskManager.Api.DTOs;
using TaskManager.Api.Services;

namespace TaskManager.Tests
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _mockService;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _mockService = new Mock<ITaskService>();
            _controller = new TasksController(_mockService.Object);
        }


        [Fact]
        public async Task GetAll_Should_Return_Ok_With_Tasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Task 1", DueDate = DateTime.UtcNow.AddDays(1), Status = "Not Started" },
                new TaskItem { Id = 2, Title = "Task 2", DueDate = DateTime.UtcNow.AddDays(2), Status = "In progress" }
            };

            _mockService.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(okResult.Value);
            Assert.Equal(2, ((List<TaskItem>)returnedTasks).Count);
        }

        [Fact]
        public async Task GetTaskById_Should_Return_NotFound_When_Task_Not_Exist()
        {
            _mockService.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync((TaskItem?)null);

            var result = await _controller.GetTaskById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetTaskById_Should_Return_Task_When_Exists()
        {
            var task = new TaskItem { Id = 1, Title = "Existing", DueDate = DateTime.UtcNow.AddDays(1), Status = "Not Started" };
            _mockService.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(task);

            var result = await _controller.GetTaskById(1);

            var returned = Assert.IsType<TaskItem>(result.Value);
            Assert.Equal("Existing", returned.Title);
        }

        [Fact]
        public async Task CreateTask_Should_Return_CreatedAtAction()
        {
            var dto = new CreateTaskDTO
            {
                Title = "New Task",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = "Not Started"
            };
            var created = new TaskItem { Id = 99, Title = dto.Title, DueDate = dto.DueDate, Status = dto.Status };

            _mockService.Setup(s => s.CreateTaskAsync(dto)).ReturnsAsync(created);

            var result = await _controller.CreateTask(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<TaskItem>(createdResult.Value);
            Assert.Equal(99, returned.Id);
        }

        [Fact]
        public async Task CreateTask_Should_Return_BadRequest_When_ServiceThrows_ValidationError()
        {
            var dto = new CreateTaskDTO
            {
                Title = "Invalid",
                DueDate = DateTime.UtcNow.AddDays(-1), // Past due date
                Status = "Not Started"
            };

            _mockService
                .Setup(s => s.CreateTaskAsync(dto))
                .ThrowsAsync(new ArgumentException("Due date must be in the future."));

            var result = await _controller.CreateTask(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var error = Assert.IsType<string>(badRequest.Value);
            Assert.Equal("Due date must be in the future.", error);
        }

        [Fact]
        public async Task CreateTask_Should_Return_BadRequest_When_Status_Invalid()
        {
            var dto = new CreateTaskDTO
            {
                Title = "Invalid Status Task",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = "INVALID"
            };

            _mockService
                .Setup(s => s.CreateTaskAsync(dto))
                .ThrowsAsync(new ArgumentException("Status must be one of: Not Started, In progress, Done"));

            var result = await _controller.CreateTask(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var error = Assert.IsType<string>(badRequest.Value);
            Assert.Contains("Status must be one of", error);
        }

        [Fact]
        public async Task UpdateStatus_Should_Return_NoContent_When_Success()
        {
            _mockService.Setup(s => s.UpdateStatusAsync(1, "Done")).ReturnsAsync(true);

            var result = await _controller.UpdateStatus(1, new UpdateStatusDTO { Status = "Done" });

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateStatus_Should_Return_NotFound_When_Fail()
        {
            _mockService.Setup(s => s.UpdateStatusAsync(1, "Done")).ReturnsAsync(false);

            var result = await _controller.UpdateStatus(1, new UpdateStatusDTO { Status = "Done" });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Should_Return_NoContent_When_Success()
        {
            _mockService.Setup(s => s.DeleteTaskAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_Should_Return_NotFound_When_Fail()
        {
            _mockService.Setup(s => s.DeleteTaskAsync(1)).ReturnsAsync(false);

            var result = await _controller.Delete(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}