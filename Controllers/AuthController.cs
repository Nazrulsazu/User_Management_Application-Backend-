using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using User_management_application.Data;
using User_management_application.Models;
using User_management_application.DTOs;
using User_management_application.Services;

namespace User_management_application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public readonly ApplicationDbContext _db;
        public readonly JwtService _jwt;

        public AuthController(ApplicationDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(x => x.Username == dto.Username))
            {
                return BadRequest("Username already exists");
            }

           var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)));

            var User = new User()
            {
                Username = dto.Username,
                PasswordHash = hash,
                Role = UserRole.User
            };

            _db.Users.Add(User);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Registered" });

        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user =await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || user.IsBlocked)
            {
                return BadRequest("Invalid or Blocked User");
            }
            var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)));

            if (user.PasswordHash != hash)
            {
                return BadRequest("Invalid Credentials");
            }
            var token = _jwt.GenerateToken(user);
            return Ok(new { token,user=new{id=user.Id} });
        }

    }
}
