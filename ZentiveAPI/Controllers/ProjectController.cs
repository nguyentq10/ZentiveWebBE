using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Services.Interface; // Namespace chứa IProjectServices
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZentiveAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectServices _projectService;

        // Inject trực tiếp IProjectServices thay vì IServiceProviders

        public ProjectController(IProjectServices projectService)
        {
            _projectService = projectService;
        }

        // GET: api/Project
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var projects = await _projectService.GetAllAsync();
            return Ok(projects);
        }

        // GET: api/Project/{id}
       
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(Guid id)
        {
            var project = await _projectService.GetByIdAsync(id); // Sửa tên hàm GetIdAsync -> GetByIdAsync

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        // POST: api/Project
        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject([FromBody] Project project)
        {
            if (project == null)
            {
                return BadRequest();
            }

            // TODO: Bạn nên dùng một DTO riêng cho việc tạo mới thay vì dùng thẳng model Project
            await _projectService.CreateAsync(project);

            // Trả về 201 Created cùng với link để truy cập resource vừa tạo
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        // PUT: api/Project/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] Project project)
        {
            if (id != project.Id)
            {
                return BadRequest("Project ID mismatch.");
            }

            var result = await _projectService.UpdateAsync(project);
            if (!result)
            {
                return NotFound();
            }

            return NoContent(); // Trả về 204 No Content khi cập nhật thành công
        }

        // DELETE: api/Project/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var result = await _projectService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent(); // Trả về 204 No Content khi xóa thành công
        }
    }
}