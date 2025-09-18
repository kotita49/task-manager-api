using TaskManager.Api.Data.Entities;
using TaskManager.Api.DTOs;

namespace TaskManager.Api.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem> CreateTaskAsync(CreateTaskDTO dto);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> DeleteTaskAsync(int id);
    }
}
