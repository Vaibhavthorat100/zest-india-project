using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentManagementAPI.Common.Exceptions;
using StudentManagementAPI.Common.Security;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentManagementAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == registerDto.Username.ToLower(), cancellationToken))
            {
                throw new ConflictException("Username is already taken.");
            }

            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == registerDto.Email.ToLower(), cancellationToken))
            {
                throw new ConflictException("Email is already registered.");
            }

            var user = new ApplicationUser
            {
                Username = registerDto.Username.Trim(),
                Email = registerDto.Email.Trim(),
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return GenerateJwtResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            var input = loginDto.UsernameOrEmail.Trim().ToLower();
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username.ToLower() == input || u.Email.ToLower() == input, cancellationToken);

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid username, email, or password.");
            }

            return GenerateJwtResponse(user);
        }

        private AuthResponseDto GenerateJwtResponse(ApplicationUser user)
        {
            var secretKey = _configuration["Jwt:Key"] ?? "ZestIndiaSuperSecretKeyForJWTAuth2026StrongKeyPassphrase!";
            var issuer = _configuration["Jwt:Issuer"] ?? "ZestIndiaAPI";
            var audience = _configuration["Jwt:Audience"] ?? "ZestIndiaClient";
            var durationInMinutes = double.Parse(_configuration["Jwt:DurationInMinutes"] ?? "120");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var expiresAt = DateTime.UtcNow.AddMinutes(durationInMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDto
            {
                Token = tokenString,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = expiresAt
            };
        }
    }
}
