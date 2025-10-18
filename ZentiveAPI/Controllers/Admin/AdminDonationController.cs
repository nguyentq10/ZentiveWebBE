using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Repo;
using Services.DTO;
using Services.Interface;
using Services.Response;

namespace ZentiveAPI.Controllers.Admin
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDonationController : ControllerBase
    {
        private readonly ISiteDonationServices _donationService;

        public AdminDonationController(ISiteDonationServices donationService)
        {
            _donationService = donationService;
        }

        /// <summary>
        /// Lấy danh sách tất cả các lượt ủng hộ nền tảng (Admin only).
        /// </summary>
        [HttpGet("donations")] // Route: GET /admin/donations
        [ProducesResponseType(typeof(PaginatedListDto<AdminDonationDetailsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDonations([FromQuery] PaginationQueryParameters queryParams)
        {
            var result = await _donationService.GetAllDonationsAsync(queryParams);
            return Ok(result);
        }
    }
}
