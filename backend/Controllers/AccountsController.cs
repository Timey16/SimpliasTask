using backend.Entities;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using backend.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<UserEntity> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> CreateUser(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (viewModel.Password != viewModel.ConfirmPassword)
            {
                return BadRequest("badConfirmPassword");
            }

            UserEntity user = new()
            {
                Email = viewModel.Email,
                FullName = viewModel.FullName,
                UserName = viewModel.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, viewModel.Password);

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new AuthResponseViewModel
            {
                Result = true,
                Message = "Account created",
                Token = CreateToken(user)
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseViewModel>> LoginUser(LoginViewmodel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserEntity? loginUser = await _userManager.FindByEmailAsync(viewModel.Email);

            if (loginUser == null || !(await _userManager.CheckPasswordAsync(loginUser, viewModel.Password)))
            {
                return Unauthorized(new AuthResponseViewModel
                {
                    Result = false,
                    Message = "Invalid username and password combination",
                    Token = ""
                });
            }

            return Ok(new AuthResponseViewModel
            {
                Result = true,
                Message = "Login successful",
                Token = CreateToken(loginUser)
            });
        }

        private string CreateToken(UserEntity user)
        {
            IConfigurationSection? authSettings = _configuration.GetSection("AuthSettings");

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] securityKey = Encoding.ASCII.GetBytes(authSettings.GetSection("securityKey").Value!);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Aud, authSettings.GetSection("validAudience").Value!),
                new Claim(JwtRegisteredClaimNames.Iss, authSettings.GetSection("validIssuer").Value!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName ?? ""),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? ""),
            };

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(securityKey),
                    SecurityAlgorithms.HmacSha256
                )
            };

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
