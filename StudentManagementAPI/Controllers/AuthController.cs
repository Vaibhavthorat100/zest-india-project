using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Services;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new application administrator / user.
        /// </summary>
        /// <param name="registerDto">Registration details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registration request received for username: {Username}", registerDto.Username);
            var result = await _authService.RegisterAsync(registerDto, cancellationToken);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User registered successfully."));
        }

        /// <summary>
        /// Authenticates user and returns a signed JWT Bearer Token.
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt received for identity: {UsernameOrEmail}", loginDto.UsernameOrEmail);
            var result = await _authService.LoginAsync(loginDto, cancellationToken);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful."));
        }
    }
}
