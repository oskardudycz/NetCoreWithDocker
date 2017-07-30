using Microsoft.EntityFrameworkCore.Migrations;
using NetCoreWithDocker.Storage;
using NetCoreWithDocker.Storage.Entities;

namespace NetCoreWithDocker.Migrations
{
    public partial class InitialSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            using (var db = new TasksDbContextFactory().Create(null))
            {
                db.Tasks.AddRange(
                    new Task { Description = "Do what you have to do" },
                    new Task { Description = "Really urgent task" },
                    new Task { Description = "This one has really low priority" });
                db.SaveChanges();
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            using (var db = new TasksDbContextFactory().Create(null))
            {
                db.Tasks.RemoveRange(db.Tasks);
                db.SaveChanges();
            }
        }
    }
}
