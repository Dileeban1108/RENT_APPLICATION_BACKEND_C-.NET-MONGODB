using System.ComponentModel.DataAnnotations;
using RentApplication.Models;
using System.Linq;

namespace RentApplication.Attributes
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (RentApplicationDbContext)validationContext.GetService(typeof(RentApplicationDbContext));
            var email = value as string;

            if (context.Users.Any(u => u.Email == email))
            {
                return new ValidationResult("Email is already in use.");
            }

            return ValidationResult.Success;
        }
    }
}
