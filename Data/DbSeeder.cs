
using System.Security.Cryptography;
using System.Text;
using User_management_application.Data;
using User_management_application.Models;


namespace User_management_application.Data
{
    public class DbSeeder
    {
        public static void SeedAdmin(ApplicationDbContext db)
        {
            if (!db.Users.Any(u => u.Role == UserRole.Admin))
            {
                using var sha = SHA256.Create();
                var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes("admin123")));

                db.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = hash,
                    Role = UserRole.Admin,
                    IsBlocked = false
                });

                db.SaveChanges();
            }
        }
    }
}
