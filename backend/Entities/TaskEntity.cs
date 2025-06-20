namespace backend.Entities
{
    public class TaskEntity
    {
        public required int TaskId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Completed { get; set; }
        public Priority Priority { get; set; }
    }
}
