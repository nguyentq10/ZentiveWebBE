using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Request;
using Services.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ZentiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PledgesController : ControllerBase
    {
        private readonly IPledgeServices _pledgeService;

        public PledgesController(IPledgeServices pledgeService)
        {
            _pledgeService = pledgeService;
        }

        [HttpPost("/api/projects/{projectId}/pledges")] // <-- Route mới, ghi đè route của controller
        public async Task<IActionResult> CreatePledge(
         [FromRoute] Guid projectId, // <-- Lấy projectId từ URL
         [FromBody] PreparePledgeRequest request, // <-- Dùng DTO mới cho body
         CancellationToken cancellationToken)
        {
            var backerIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!Guid.TryParse(backerIdString, out var backerId))
            {
                return Unauthorized(new ProblemDetails { Title = "Invalid user identifier." });
            }

            try
            {
                var response = await _pledgeService.PreparePledgeAsync(projectId, request, backerId, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = "Pledge preparation failed", Detail = ex.Message });
            }
        }
    }
}
