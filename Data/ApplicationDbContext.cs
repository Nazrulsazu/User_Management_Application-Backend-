using Microsoft.EntityFrameworkCore;
using User_management_application.Models;

namespace User_management_application.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

    public DbSet<User> Users { get; set; }
    }
}
