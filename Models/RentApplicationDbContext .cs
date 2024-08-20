using Microsoft.EntityFrameworkCore;
using RentApplication.Models;

namespace RentApplication.Models
{
    public class RentApplicationDbContext : DbContext
    {
        public RentApplicationDbContext(DbContextOptions<RentApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
