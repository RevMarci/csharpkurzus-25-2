using Microsoft.AspNetCore.Mvc;
using quiz.Models;
using quiz.Services;

namespace quiz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult register([FromBody] AuthRequest request)
        {
            var success = _authService.register(request.Username, request.Password);

            if (!success)
            {
                return BadRequest("Username already taken.");
            }

            return Ok("Registration successful.");
        }

        [HttpPost("login")]
        public IActionResult login([FromBody] AuthRequest request)
        {
            var success = _authService.login(request.Username, request.Password);

            if (!success)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok("Login successful!");
        }
    }
}
