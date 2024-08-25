using Microsoft.EntityFrameworkCore;

namespace RentApplication.Models
{
    public class RentApplicationDbContext : DbContext
    {
        public RentApplicationDbContext(DbContextOptions<RentApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BookedVehicles> VehicleBookings { get; set; }

    }
}
