namespace backend.Entities
{
    public class TaskEntity
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsComplete { get; set; }
        public Priority Priority { get; set; }
        public Guid UserId { get; set; }
    }
}
