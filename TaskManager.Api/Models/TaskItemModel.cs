using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models
{
    public class TaskItemModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Not Started";

        public DateTime DueDate { get; set; }
    }
}
