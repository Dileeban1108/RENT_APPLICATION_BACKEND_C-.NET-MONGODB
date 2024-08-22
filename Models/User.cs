using System.ComponentModel.DataAnnotations;
using RentApplication.Attributes;

namespace RentApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [UniqueEmail]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        [UniquePhoneNumber] 
        public string PhoneNumber { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
