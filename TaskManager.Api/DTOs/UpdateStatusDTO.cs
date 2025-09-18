using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.DTOs
{
    public class UpdateStatusDTO
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
