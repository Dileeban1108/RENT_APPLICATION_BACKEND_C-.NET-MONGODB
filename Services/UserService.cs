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

        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving users: {ex.Message}");
                throw;
            }
        }

        public async Task<User> GetUserAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user with email {email}: {ex.Message}");
                throw;
            }
        }

        public async Task CreateUserAsync(User newUser)
        {
            try
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                throw;
            }
        }

        public async Task BookVehicleAsync(BookedVehicles booking)
        {
            try
            {
                _context.VehicleBookings.Add(booking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error booking vehicle: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateUserAsync(int id, User updatedUser)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<User> Authenticate(string email, string password)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
                if (user == null || !VerifyPassword(password, user.Password))
                {
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error authenticating user with email {email}: {ex.Message}");
                throw;
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying password: {ex.Message}");
                throw;
            }
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[32];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating refresh token: {ex.Message}");
                throw;
            }
        }
    }
}
