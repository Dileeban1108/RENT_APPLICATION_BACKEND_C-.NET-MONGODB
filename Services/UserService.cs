using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentApplication.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RentApplication.Services
{
    public class UserService
    {
        private readonly RentApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(RentApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<User>> GetUsersAsync() =>
            await _context.Users.ToListAsync();

        public async Task<User> GetUserAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<User> GetUserByEmailAsync(string email) =>
            await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

        public async Task CreateUserAsync(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateUserAsync(int id, User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found");
            }

            // Update properties
            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;
            // Add other properties as needed

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<string> Authenticate(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.Password))
            {
                return null;
            }

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPassword);
        }

    }

}
