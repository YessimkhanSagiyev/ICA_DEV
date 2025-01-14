using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThAmCo.Main.DTOs;
using ThAmCo.Main.Models;
using ThAmCo.Main.Services.UserService;

namespace ThAmCo.Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Address = userDto.Address,
                CreatedAt = DateTime.UtcNow
            };

            await _userService.AddUser(user);

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address
            };

            return Ok(response);
        }

        [HttpPost("login")]
        [Authorize]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var isValid = await _userService.ValidateUserCredentials(loginDto.Email, loginDto.Password);
            if (!isValid)
                return Unauthorized("Invalid credentials.");

            var user = await _userService.GetAllUsers()
                                         .ContinueWith(t => t.Result.FirstOrDefault(u => u.Email == loginDto.Email));

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound();

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address
            };

            return Ok(response);
        }
    }
}

