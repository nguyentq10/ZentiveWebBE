using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Repo;
using Services.Interface; // Namespace chứa IProjectServices
using Services.Request;
using Services.Response;
using Services.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ZentiveAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectServices _projectService;
        private readonly IPledgeServices _pledgeService;
        public ProjectController(IProjectServices projectService, IPledgeServices pledgeService)
        {
            _projectService = projectService; 
            _pledgeService = pledgeService;
        }
        [HttpGet] 
        public async Task<ActionResult<PaginatedProjectResponse>> QueryProjects([FromQuery] QueryProjectsRequest request)
        {
            var result = await _projectService.QueryProjectsAsync(request);
            return Ok(result);
        }
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProjectDetailResponseDto>> GetProject(Guid id) // <-- Thay đổi kiểu trả về
        {
            var project = await _projectService.GetByIdAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            var projectDto = new ProjectDetailResponseDto
            {
                Id = project.Id,
                Title = project.Title,
                Slug = project.Slug,
                Summary = project.Summary,
                Goal = project.Goal,
                EndAt = project.EndAt,
                Status = project.Status,
                CategoryId = project.CategoryId,
                CreatorId = project.CreatorId,
                CreatedAt = project.CreatedAt,
                MediaCoverUrl = project.MediaCoverUrl,
                Description = project.Description,
            };

            return Ok(projectDto);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProjectDetailResponseDto>> CreateProject([FromBody] CreateProjectRequestDto request)
        {


            // Lấy ID của người dùng đang thực hiện request từ token
            var creatorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            Console.WriteLine("Creator ID from token: " + creatorIdString); // Debug log
            if (string.IsNullOrEmpty(creatorIdString))
            {
                return Unauthorized();
            }

            var creatorId = new Guid(creatorIdString);

            // Gọi service để tạo dự án
            var createdProject = await _projectService.CreateDraftProjectAsync(request, creatorId);

            // Trả về 201 Created cùng với thông tin chi tiết và link đến resource vừa tạo
            return CreatedAtAction(nameof(GetProject), new { id = createdProject.Id }, createdProject);
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
        {
            // Lấy ID của người dùng đang thực hiện request từ token
            var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserIdString))
            {
                return Unauthorized(); // Token không hợp lệ
            }
            var currentUserId = new Guid(currentUserIdString);

            try
            {
                var success = await _projectService.UpdateProjectAsync(id, request, currentUserId);
                if (!success)
                {
                    return NotFound(); // Không tìm thấy project với ID này
                }

                return NoContent(); // 204 No Content - Cập nhật thành công
            }
            catch (UnauthorizedAccessException ex)
            {
                // Bắt lỗi không có quyền từ service
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (InvalidOperationException ex)
            {
                // Bắt lỗi sai trạng thái từ service
                return BadRequest(ex.Message); // 400 Bad Request
            }
        }
        [HttpPost("{id}/submit")]
        [Authorize] // Yêu cầu người dùng phải đăng nhập
        public async Task<IActionResult> SubmitProject(Guid id)
        {
            // Lấy ID của người dùng từ token
            var creatorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(creatorIdString))
            {
                return Unauthorized();
            }
            var creatorId = new Guid(creatorIdString);

            try
            {
                // Gọi service để thực hiện logic
                await _projectService.SubmitProjectForApprovalAsync(id, creatorId);

                // Trả về 204 No Content khi thành công
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                // Bắt lỗi nếu không tìm thấy dự án
                return NotFound(ex.Message); // 404 Not Found
            }
            catch (UnauthorizedAccessException ex)
            {
                // Bắt lỗi nếu người dùng không phải chủ sở hữu
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (InvalidOperationException ex)
            {
                // Bắt lỗi nếu trạng thái dự án không hợp lệ
                return BadRequest(ex.Message); // 400 Bad Request
            }
        }
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền truy cập
        public async Task<ActionResult<PaginatedPendingProjectResponse>> GetPendingProjects(
            [FromQuery] AdminQueryPendingProjectsRequest request)
        {
            var result = await _projectService.GetPendingProjectsAsync(request);
            return Ok(result);
        }

        [HttpGet("{slug}")]
        [AllowAnonymous] 
        public async Task<ActionResult<ProjectDetailResponseDto>> GetProjectBySlug(string slug)
        {
            var projectDetail = await _projectService.GetPublishedProjectBySlugAsync(slug);

            if (projectDetail == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy dự án
            }

            return Ok(projectDetail);
        }
        [HttpGet("{projectId}/pledges")]
        [Authorize(Roles = "Admin, Creator")] // Yêu cầu đăng nhập, logic chi tiết sẽ do service xử lý
        public async Task<IActionResult> GetPledgesForProject(
        [FromRoute] Guid projectId,
        [FromQuery] PledgeQueryParameters queryParams)
        {
            try
            {
                // Lấy thông tin người dùng đang đăng nhập
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(userIdString, out var currentUserId);
                var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "";

                // Gọi service để lấy dữ liệu
                var result = await _pledgeService.GetPledgesForProjectAsync(projectId, queryParams, currentUserId, userRole);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Dùng 403 Forbidden khi người dùng không có quyền
                return Forbid();
            }
        }
    }
}