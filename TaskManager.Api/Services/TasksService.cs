using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Data.Entities;
using TaskManager.Api.DTOs;

namespace TaskManager.Api.Services
{
    public class TasksService : ITaskService
    {
        private readonly AppDbContext _db;
        private static readonly string[] AllowedStatuses = new[] { "Not Started", "In progress", "Done" };
        public TasksService(AppDbContext db)
        {
            _db = db;

        }
        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _db.Tasks.OrderBy(t => t.DueDate).ToListAsync();
        }
        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _db.Tasks.FindAsync(id);
        }

        public async Task<TaskItem> CreateTaskAsync(CreateTaskDTO dto)
        {
            if (dto.DueDate <= DateTime.UtcNow) throw new ArgumentException("Due date must be in the future.");
            var status = string.IsNullOrWhiteSpace(dto.Status) ? "Not Started" : dto.Status;
            if (!AllowedStatuses.Contains(status)) throw new ArgumentException($"Status must be one of: {string.Join(", ", AllowedStatuses)}");
            var task = new TaskItem
            {
                Title = dto.Title,
                DueDate = dto.DueDate,
                Description = dto.Description,
                Status = status,
            };
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            if (!AllowedStatuses.Contains(status)) return false;
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return false;
            task.Status = status;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return false;
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
