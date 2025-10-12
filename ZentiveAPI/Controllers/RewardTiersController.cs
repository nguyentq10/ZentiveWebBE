using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Request;
using Services.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ZentiveAPI.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/tiers")] // URL lồng nhau
    public class RewardTiersController : ControllerBase
    {
        private readonly IProjectServices _projectService;

        public RewardTiersController(IProjectServices projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Tạo một gói thưởng mới cho một dự án.
        /// </summary>
        [HttpPost]
        [Authorize] // Yêu cầu người dùng phải đăng nhập
        public async Task<ActionResult<RewardTierResponseDto>> CreateTier(
            [FromRoute] Guid projectId,
            [FromBody] CreateRewardTierRequestDto request)
        {
            // Lấy ID của người dùng từ token đã được xác thực
            var creatorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            Console.WriteLine("Creator ID from token: " + creatorIdString); // Debug log
            if (string.IsNullOrEmpty(creatorIdString))
            {
                return Unauthorized();
            }
            var creatorId = new Guid(creatorIdString);

            try
            {
                // Gọi service để thực hiện logic nghiệp vụ
                var createdTier = await _projectService.CreateTierForProjectAsync(projectId, request, creatorId);

                // Trả về 201 Created cùng với dữ liệu của gói thưởng vừa tạo
                return Created($"/api/projects/{projectId}/tiers/{createdTier.Id}", createdTier);
            }
            catch (KeyNotFoundException ex)
            {
                // Bắt lỗi nếu không tìm thấy dự án
                return NotFound(ex.Message); // 404 Not Found
            }
            catch (UnauthorizedAccessException ex)
            {
                // Bắt lỗi nếu người dùng không phải chủ sở hữu dự án
                return Forbid(ex.Message); // 403 Forbidden
            }
        }
    }
}
