using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Request;
using Services.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ZentiveAPI.Controllers.Me
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly IProjectServices _projectService;
        public MeController(IProjectServices projectService)
        {
            _projectService = projectService;
        }
        [HttpGet("projects")]
        public async Task<ActionResult<PaginatedMyProjectsDashboardResponse>> GetMyProjects( // <-- Cập nhật kiểu trả về
    [FromQuery] MyProjectsQueryRequest request)
        {
            var creatorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(creatorIdString))
            {
                return Unauthorized();
            }
            var creatorId = new Guid(creatorIdString);

            var result = await _projectService.GetMyProjectsAsync(creatorId, request);
            return Ok(result);
        }
    }
}
