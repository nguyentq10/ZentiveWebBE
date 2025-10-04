using DAL.DBcontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Services.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        public ProjectController(IServiceProviders serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // GET: api/project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _serviceProvider.ProjectServices.GetAllAsync();
        }

        // GET: api/project/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(Guid id)
        {
            var project = await _serviceProvider.ProjectServices.GetIdAsync(id);
              

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        //// POST: api/project
        //[HttpPost]
        //public async Task<ActionResult<Project>> CreateProject(Project project)
        //{
        //    project.Id = Guid.NewGuid();
        //    project.CreatedAt = DateTime.UtcNow;
        //    _context.Projects.Add(project);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        //}

        //// PUT: api/project/{id}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateProject(Guid id, Project project)
        //{
        //    if (id != project.Id)
        //    {
        //        return BadRequest();
        //    }

        //    project.UpdatedAt = DateTime.UtcNow;
        //    _context.Entry(project).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!_context.Projects.Any(p => p.Id == id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// DELETE: api/project/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteProject(Guid id)
        //{
        //    var project = await _context.Projects.FindAsync(id);
        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Projects.Remove(project);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}
