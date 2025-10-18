using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Request;
using Services.Response;

namespace ZentiveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountServices _userService; // Hoặc IAccountService

        public AccountsController(IAccountServices userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy tổng số lượng người dùng đang hoạt động.
        /// </summary>
        [HttpGet("count")] // Route: GET /api/users/count
        [AllowAnonymous] // Hoặc [Authorize(Roles = "Admin")] tùy yêu cầu
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserCount()
        {
            var count = await _userService.GetTotalUserCountAsync();
            return Ok(count); // Trả về số nguyên
        }
    }
}
