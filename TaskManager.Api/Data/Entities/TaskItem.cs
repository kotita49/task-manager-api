namespace TaskManager.Api.Data.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "Not Started";
        public DateTime DueDate { get; set; }
    }
}
