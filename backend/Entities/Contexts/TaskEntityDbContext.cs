using Microsoft.EntityFrameworkCore;

namespace backend.Entities.Contexts
{
    public class TaskEntityDbContext : DbContext
    {
        public DbSet<TaskEntity> Tasks { get; set; }

        public TaskEntityDbContext(DbContextOptions<TaskEntityDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskEntity>()
                .Property(b => b.Name)
                .IsRequired();

            modelBuilder.Entity<TaskEntity>()
                .Property(b => b.Description)
                .IsRequired();

            modelBuilder.Entity<TaskEntity>()
                .Property(b => b.TaskId)
                .IsRequired();

            modelBuilder.Entity<TaskEntity>()
                .Property(b => b.Completed)
                .HasColumnType("BOOLEAN");

            modelBuilder.Entity<TaskEntity>()
                .Property(b => b.Priority)
                .HasDefaultValue(Priority.UNSET)
                .IsRequired();

            modelBuilder.Entity<TaskEntity>()
                .Property(b => b.CreationDate)
                .IsRequired();

            modelBuilder.Entity<TaskEntity>()
                .HasKey(b => b.TaskId);
        }
    }
}
