using Microsoft.EntityFrameworkCore;
using NetCoreWithDocker.Storage.Entities;

namespace NetCoreWithDocker.Storage
{
    public class TasksDbContext : DbContext
    {
        public TasksDbContext(DbContextOptions<TasksDbContext> options)
            : base(options)
        { }

        public DbSet<Task> Tasks { get; set; }
    }
}
