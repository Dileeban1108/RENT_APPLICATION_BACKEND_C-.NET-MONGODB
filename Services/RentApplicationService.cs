using RentApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace RentApplication.Services
{
    public class RentApplicationService
    {
        private readonly RentApplicationDbContext _dbContext;

        public RentApplicationService(RentApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create
        public async Task CreateUserAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        // Read
        public async Task<List<User>> GetUsersAsync() =>
            await _dbContext.Users.ToListAsync();

        public async Task<User?> GetUserAsync(string id) =>
            await _dbContext.Users.FindAsync(id);

        // Update
public async Task UpdateUserAsync(string id, User updatedUser)
{
    var user = await _dbContext.Users.FindAsync(id);
    if (user == null) return;

    user.Name = updatedUser.Name;
    user.Email = updatedUser.Email;
    user.PhoneNumber = updatedUser.PhoneNumber;

    await _dbContext.SaveChangesAsync();
}


        // Delete
        public async Task DeleteUserAsync(string id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
