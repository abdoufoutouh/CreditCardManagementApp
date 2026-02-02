using CreditCardManagementApp.DTOS;
using CreditCardManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardManagementApp.Controllers
{
    /// <summary>
    /// Controller for handling authentication endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">User registration data</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Invalid input data",
                    Data = null
                });
            }

            try
            {
                // Attempt to register user
                var response = await _authService.SignupAsync(request);

                if (response == null)
                {
                    return Conflict(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Message = "User with this email already exists",
                        Data = null
                    });
                }

                return CreatedAtAction(nameof(Signup), new ApiResponse<AuthResponse>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Login an existing user
        /// </summary>
        /// <param name="request">User login credentials</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Invalid input data",
                    Data = null
                });
            }

            try
            {
                // Attempt to authenticate user
                var response = await _authService.LoginAsync(request);

                if (response == null)
                {
                    return Unauthorized(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Message = "Invalid email or password",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<AuthResponse>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Logout endpoint (stateless - client should discard token)
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // For stateless JWT, logout is handled client-side by discarding the token
            // This endpoint provides a consistent API structure
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Logout successful. Please discard your token on the client side.",
                Data = null
            });
        }
    }
}
