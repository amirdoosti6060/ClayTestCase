using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserWebAPI.Interfaces;
using UserWebAPI.Models;

namespace UserWebAPI.Services
{
    public class AuthenticatorService : IAuthenticatorService
    {
        private readonly UserDbContext _dbContext;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthenticatorService> _logger;

        public AuthenticatorService(
            UserDbContext dbContext, 
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthenticatorService> logger)
        {
            _dbContext = dbContext;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<GeneralResponse> Login(LoginRequest loginRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            var user = await Authenticate(loginRequest);

            if (user != null)
            {
                response.Data = GenerateToken(user);
            }
            else
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"User {loginRequest.Email} not found or password is not valid!";
            }

            return response;
        }

        private async Task<User?> Authenticate(LoginRequest loginRequest)
        {
            var user = await _dbContext.Users.Where(
                e => e.Email.ToLower() == loginRequest.Email!.ToLower() &&
                e.Password == loginRequest.Password).FirstOrDefaultAsync();

            return user;
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            int.TryParse(_jwtSettings.AccessTokenValidityInMinute, out int validityInMinute);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FullName),
                //new Claim(ClaimTypes.Role, user.Role),
                new Claim("Role", user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(validityInMinute),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
