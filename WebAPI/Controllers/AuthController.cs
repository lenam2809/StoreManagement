using Application.DTOs.Auth;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;

        public IConfiguration _configuration { get; }

        public AuthController(IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
        {
            try
            {
                string role = UserRoles.Admin;
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid login request");
                }

                var userExists = await _userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return BadRequest("User already exists");

                User user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };
                var createUserResult = await _userManager.CreateAsync(user, model.Password);
                if (!createUserResult.Succeeded)
                    return BadRequest("User creation failed! Please check user details and try again.");

                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                if (await _roleManager.RoleExistsAsync(role))
                    await _userManager.AddToRoleAsync(user, role);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid login request");
                }

                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                    };

                    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
                    _ = int.TryParse(_configuration["Jwt:ExpiresInMinutes"], out int tokenExpiresInMinutes);
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:ValidIssuer"],
                        audience: _configuration["Jwt:ValidAudience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(tokenExpiresInMinutes),
                        signingCredentials: credentials);

                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
