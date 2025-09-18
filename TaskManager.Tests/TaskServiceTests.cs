using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Api.Data;
using TaskManager.Api.Data.Entities;
using TaskManager.Api.DTOs;
using TaskManager.Api.Services;

namespace TaskManager.Tests
{
    public class TaskServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateTaskAsync_AddsTask()
        {
            // Arrange
            var db = GetDbContext();
            var service = new TasksService(db);
            var dto = new CreateTaskDTO
            {
                Title = "New Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = "Not Started"
            };
            // Act
            var task = await service.CreateTaskAsync(dto);
            // Assert
            Assert.NotNull(task);
            Assert.Equal("New Task", task.Title);
            Assert.Single(db.Tasks);
        }

        [Fact]
        public async Task CreateTask_Should_Throw_When_Due_Date_Past()
        {
            var db = GetDbContext();
            var service = new TasksService(db);
            var dto = new CreateTaskDTO
            {
                Title = "New Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(-1),
                Status = "Not Started"
            };
            
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTaskAsync(dto));
        }

        [Fact]
        public async Task CreateTask_Should_Throw_When_Status_Invalid()
        {
            var db = GetDbContext();
            var service = new TasksService(db);

            var dto = new CreateTaskDTO
            {
                Title = "Bad Status",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = "Invalid"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTaskAsync(dto));

        }

        [Fact]
        public async Task GetTaskById_Should_Return_Task_When_Found()
        {
            var db = GetDbContext();
            db.Tasks.Add(new TaskItem
            {
                Title = "Test",
                DueDate = DateTime.UtcNow.AddDays(2),
                Status = "Not Started"
            });
            await db.SaveChangesAsync();

            var service = new TasksService(db);
            var task = await service.GetTaskByIdAsync(1);

            Assert.NotNull(task);
            Assert.Equal("Test", task!.Title);
        }

        [Fact]
        public async Task UpdateStatus_Should_Update_When_Valid()
        {
            var db = GetDbContext();
            var task = new TaskItem
            {
                Title = "Update me",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = "Not Started"
            };
            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            var service = new TasksService(db);
            var result = await service.UpdateStatusAsync(task.Id, "Done");

            Assert.True(result);
            var updated = await db.Tasks.FindAsync(task.Id);
            Assert.Equal("Done", updated!.Status);
        }

        [Fact]
        public async Task UpdateStatus_Should_ReturnFalse_When_InvalidStatus()
        {
            var db = GetDbContext();
            var task = new TaskItem
            {
                Title = "Invalid update",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = "Not Started"
            };
            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            var service = new TasksService(db);
            var result = await service.UpdateStatusAsync(task.Id, "INVALID");

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteTask_Should_Remove_Task()
        {
            var db = GetDbContext();
            var task = new TaskItem
            {
                Title = "Delete me",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = "Not Started"
            };
            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            var service = new TasksService(db);
            var result = await service.DeleteTaskAsync(task.Id);

            Assert.True(result);
            Assert.Empty(db.Tasks);
        }

        [Fact]
        public async Task DeleteTask_Should_ReturnFalse_When_NotFound()
        {
            var db = GetDbContext();
            var service = new TasksService(db);

            var result = await service.DeleteTaskAsync(123);
            Assert.False(result);
        }
    }
}
