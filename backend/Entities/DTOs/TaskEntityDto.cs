namespace backend.Entities.DTOs
{
    public class TaskEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsComplete { get; set; }
        public string Priority { get; set; }
    }
}
