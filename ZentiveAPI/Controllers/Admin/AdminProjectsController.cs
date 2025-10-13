using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ZentiveAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/projects")]
    [Authorize(Roles = "Admin")]
    public class AdminProjectsController : ControllerBase
    {
        private readonly IProjectServices _projectService;

        public AdminProjectsController(IProjectServices projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveProject(Guid id)
        {
            // Lấy ID của Admin đang thực hiện request từ token
            var adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(adminIdString))
            {
                return Unauthorized();
            }
            var adminId = new Guid(adminIdString);

            try
            {
                // Truyền adminId vào service
                await _projectService.ApproveProjectAsync(id, adminId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
