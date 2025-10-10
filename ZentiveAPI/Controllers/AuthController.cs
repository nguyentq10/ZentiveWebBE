using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Request;
using Services.Response;

namespace ZentiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;

        // Inject IAccountServices vào controller
        public AuthController(IAuthServices authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Endpoint để đăng ký tài khoản mới.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var token = await _authService.RegisterAsync(request);

                // Trả về token trong một đối tượng JSON
                return Ok(new RegisterResponse { Token = token });
            }
            catch (Exception ex)
            {
                // Bắt lỗi "Email already registered." từ service
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint để đăng nhập.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(string), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var loginResponse = await _authService.LoginAsync(request);

            if (loginResponse == null)
            {
                // Nếu service trả về null, thông tin đăng nhập không hợp lệ
                return Unauthorized("Invalid email or password.");
            }

            // Trả về đối tượng LoginResponse (chứa token và thông tin user)
            return Ok(loginResponse);
        }
    }
}
