using Microsoft.EntityFrameworkCore;
using TaskMatrix.Domain.Entities;
using TaskMatrix.Domain.Enums;


namespace TaskMatrix.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}

        public DbSet<AppTask> AppTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppTask>().HasKey(p => p.Id);
            // Seed data
            modelBuilder.Entity<Domain.Entities.AppTask>().HasData(
                new AppTask(1,"Test Task 1", "Description 1", TaskPriority.High,new DateTime(), AppTaskStatus.Pending) ,
                new AppTask(2, "Test Task 2", "Description 2", TaskPriority.Low, new DateTime(),AppTaskStatus.Pending)
            );
        }
    }
}
