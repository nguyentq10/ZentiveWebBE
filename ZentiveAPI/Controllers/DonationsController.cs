using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Request;
using System.Security.Claims;

namespace ZentiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly ISiteDonationServices _donationService;

        public DonationsController(ISiteDonationServices donationService)
        {
            _donationService = donationService;
        }

        [HttpPost]
        [AllowAnonymous] // Cho phép tất cả mọi người truy cập
        public async Task<IActionResult> CreateDonation([FromBody] CreateDonationRequestDto request, CancellationToken cancellationToken)
        {
            Guid? userId = null;
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            try
            {
                var response = await _donationService.PrepareDonationAsync(request, userId, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = "Donation preparation failed", Detail = ex.Message });
            }
        }
    }
}
