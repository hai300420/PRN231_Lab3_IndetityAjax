using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAccountRepository _accountRepository;

        public AuthController(IConfiguration config, IAccountRepository accountRepository)
        {
            _config = config;
            _accountRepository = accountRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var account = _accountRepository.ValidateLogin(request.Email, request.Password);
            if (account == null)
                return Unauthorized("Invalid email or password");

            var token = GenerateJwtToken(account);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (_accountRepository.GetAccountByEmail(request.Email) != null)
                return Conflict("Email is already registered.");

            var newAccount = new Account
            {
                AccountName = request.AccountName,
                Email = request.Email,
                Password = request.Password,
                RoleId = 2 // Default to 'Customer'
            };

            _accountRepository.SaveAccount(newAccount);
            return Ok("Registration successful");
        }

        [Authorize]
        [HttpGet("my-token")]
        public IActionResult GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
                return Unauthorized();

            var userClaims = identity.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(userClaims);
        }

        private string GenerateJwtToken(Account account)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var claims = new[]
            {
            new Claim(ClaimTypes.Email, account.Email!),
            new Claim(ClaimTypes.Name, account.AccountName ?? account.Email!),
            new Claim(ClaimTypes.Role, account.Role?.RoleName ?? "Customer"),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string AccountName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

}

