using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using User_management_application.Data;
using User_management_application.Models;
using User_management_application.DTOs;

namespace User_management_application.Controllers
{
    [ApiController]
   
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        public readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int GetCurrentUserId(ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new InvalidOperationException("User ID claim not found.");

            return int.Parse(idClaim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _db.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _db.Users.FindAsync(id);
            return Ok(user);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}/promote")]
        public async Task<IActionResult> Promote(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            user.Role = UserRole.Admin;
            await _db.SaveChangesAsync();
            return Ok(new { message = "User Promote to Admin" });
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}/demote")]
        public async Task<IActionResult> Demote(int id)
        {
            var currentUserId = GetCurrentUserId(User);
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id == currentUserId)
            {
                var admincount = await _db.Users.CountAsync(u => u.Role == UserRole.Admin && u.Id != user.Id);
                if (admincount == 0)
                {
                    return BadRequest("You are the last admin");
                }

            }
            user.Role = UserRole.User;
            await _db.SaveChangesAsync();
            return Ok(new { message = "User is demoted" });
        }
        [HttpPut("{id}/block")]
        public async Task<IActionResult> Block(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsBlocked = true;
            await _db.SaveChangesAsync();
            return Ok(new { message = "User blocked" });
        }

        [HttpPut("{id}/unblock")]
        public async Task<IActionResult> Unblock(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsBlocked = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "User unblocked" });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok(new { message = "User deleted" });
        }
        [HttpPut("{id}/update")]
        public async Task<IActionResult> Update(int id,[FromBody] UpdateUserDto dto)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            await _db.SaveChangesAsync();
            return Ok(new { message = "User info updated" });
        }

    }
}
