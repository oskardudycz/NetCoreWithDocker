using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreWithDocker.Storage;
using NetCoreWithDocker.Storage.Entities;
using Threading = System.Threading.Tasks;

namespace NetCoreWithDocker.Controllers
{
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly TasksDbContext dbContext;

        private DbSet<Task> Tasks => dbContext?.Tasks;

        public TasksController(TasksDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
        }

        // GET api/tasks
        [HttpGet]
        public async Threading.Task<IActionResult> Get()
        {
            return Ok(await Tasks.ToListAsync());
        }

        // GET api/tasks/5
        [HttpGet("{id}")]
        public async Threading.Task<IActionResult> Get(int id)
        {
            var result = await Tasks.FindAsync(id);

            if (result == null)
                return NotFound(id);

            return Ok(result);
        }

        // POST api/tasks
        [HttpPost]
        public async Threading.Task<IActionResult> Post([FromBody]Task task)
        {
            if (Tasks.Any(t => t.Id == task.Id))
                return Forbid();

            Tasks.Add(task);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        // PUT api/tasks/5
        [HttpPut("{id}")]
        public async Threading.Task<IActionResult> Put(int id, [FromBody]Task task)
        {
            if (await Tasks.AllAsync(t => t.Id != id))
                return NotFound(task.Id);

            Tasks.Update(task);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        // DELETE api/tasks/5
        [HttpDelete("{id}")]
        public async Threading.Task<IActionResult> Delete(int id)
        {
            var result = await Tasks.FindAsync(id);

            if (result == null)
                return NotFound(id);

            Tasks.Remove(result);

            return Ok();
        }
    }
}