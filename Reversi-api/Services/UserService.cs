using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reversi_api.Data;
using Reversi_api.Models;
using Reversi_api.Resources;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Reversi_api.Services
{
    public sealed class UserService : IUserService
    {
        private readonly ReversiContext _context;
        private readonly string _pepper;
        private readonly int _iteration = 3;
        private IConfiguration _config;

        public UserService(ReversiContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
            _pepper = Environment.GetEnvironmentVariable("PasswordHash") ?? throw new InvalidOperationException("PasswordHash not found.");
        }

        public async Task<UserResource> Register(RegisterResource resource, CancellationToken cancellationToken)
        {
            var user = new Player
            {
                Name = resource.Name,
                Email = resource.Email,
                PasswordSalt = PasswordHasher.GenerateSalt()
            };
            user.Password = PasswordHasher.ComputeHash(resource.Password, user.PasswordSalt, _pepper, _iteration);
            await _context.Player.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var token = GenerateJSONWebToken(user);

            return new UserResource(user.Id, user.Name, user.Email, token);
        }

        public async Task<UserResource> Login(LoginResource resource, CancellationToken cancellationToken)
        {
            var user = await _context.Player
                .FirstOrDefaultAsync(x => x.Name == resource.Name || x.Email == resource.Email, cancellationToken);

            if (user == null)
                throw new Exception("Username or password did not match.");

            var passwordHash = PasswordHasher.ComputeHash(resource.Password, user.PasswordSalt, _pepper, _iteration);
            if (user.Password != passwordHash)
                throw new Exception("Username or password did not match.");

            var token = GenerateJSONWebToken(user);

            return new UserResource(user.Id, user.Name, user.Email, token);
        }

        private string GenerateJSONWebToken(Player userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, userInfo.Email),
                new(JwtRegisteredClaimNames.Name, userInfo.Name)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
