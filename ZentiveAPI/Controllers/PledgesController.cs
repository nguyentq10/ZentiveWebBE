using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Request;
using Services.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging; // <-- THÊM USING NÀY
namespace ZentiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PledgesController : ControllerBase
    {
        private readonly IPledgeServices _pledgeService;
        private readonly ILogger<PledgesController> _logger; // <-- THÊM DÒNG NÀY
        public PledgesController(IPledgeServices pledgeService, ILogger<PledgesController> logger)
        {
            _pledgeService = pledgeService;
            _logger = logger; // <-- THÊM DÒNG NÀY
        }

        [HttpPost("projects/{projectId}/pledges")]
        public async Task<IActionResult> CreatePledge(
            [FromRoute] Guid projectId,
            [FromBody] PreparePledgeRequest request,
            CancellationToken cancellationToken)
        {
            var backerIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(backerIdString, out var backerId))
            {
                return Unauthorized(new ProblemDetails { Title = "Invalid user identifier in token." });
            }

            try
            {
                // Bỏ HttpContext
                var response = await _pledgeService.PreparePledgeAsync(projectId, request, backerId, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // === SỬA LẠI KHỐI CATCH NÀY ===

                // 1. Ghi log lỗi ra console (với dấu ### để dễ thấy)
                _logger.LogError(ex, "### LỖI 500 KHI GỌI PREPARE PLEDGE ###");

                // 2. Trả về chi tiết lỗi trong response (CHỈ DÙNG ĐỂ DEBUG)
                return StatusCode(500, new
                {
                    title = "An unexpected error occurred. See details.",
                    status = 500,
                    detail = ex.Message, // <-- Chi tiết lỗi
                    innerException = ex.InnerException?.Message, // Lỗi bên trong
                    stackTrace = ex.StackTrace.ToString() // Dấu vết lỗi
                });
            }
        }
    }
}
