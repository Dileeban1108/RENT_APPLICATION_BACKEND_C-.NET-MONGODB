using Microsoft.AspNetCore.Mvc;
using RentApplication.Models;
using RentApplication.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RentApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UsersController> _logger;
        public UsersController(ILogger<UsersController> logger, UserService userService)
        {
                _logger = logger;
            _userService = userService;
        }
    [HttpPost("bookVehicle")]
public async Task<IActionResult> BookVehicle([FromBody] BookedVehicles booking)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Model state is invalid: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return BadRequest(ModelState);
        }

        var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        if (userName == null || userEmail == null)
        {
            _logger.LogWarning("Unauthorized access attempt by user with email: {Email}", userEmail);
            return Unauthorized();
        }

        booking.UserName = userName;
        booking.UserEmail = userEmail;

        try
        {
            await _userService.BookVehicleAsync(booking);
            _logger.LogInformation("Booking confirmed successfully for user: {UserName}", userName);
            return Ok(new { message = "Booking confirmed successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while booking vehicle");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }
        [HttpGet]
        public async Task<ActionResult<List<User>>> Get() =>
            Ok(await _userService.GetUsersAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<User>> GetByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User newUser)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.CreateUserAsync(newUser);
            return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.Authenticate(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var accessToken = _userService.GenerateJwtToken(user);
            var refreshToken = _userService.GenerateRefreshToken();

            return Ok(new
            {
                token = accessToken,
                refreshToken = refreshToken,
                userId = user.Id  // Include User ID in the response
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User updatedUser)
        {
            try
            {
                await _userService.UpdateUserAsync(id, updatedUser);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }


    }
}
