using System.Net.Mail;

namespace User_management_application.Models
{
    public enum UserRole
    {
        User,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsBlocked { get; set; } = false;
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";

    }
}