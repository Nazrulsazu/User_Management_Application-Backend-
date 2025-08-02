using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using User_management_application.Models;
namespace User_management_application.Services

{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role == UserRole.Admin? "Admin":"User")

                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                 issuer : "UserManagementAPI",
            audience : "UserManagementClient", claims: claims, expires: DateTime.UtcNow.AddMinutes(30),signingCredentials:creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        } 
    }
}
