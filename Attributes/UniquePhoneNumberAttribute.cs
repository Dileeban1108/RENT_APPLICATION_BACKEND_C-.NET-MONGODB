using System.ComponentModel.DataAnnotations;
using RentApplication.Models;
using System.Linq;

namespace RentApplication.Attributes
{
    public class UniquePhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (RentApplicationDbContext)validationContext.GetService(typeof(RentApplicationDbContext));
            var phoneNumber = value as string;

            if (context.Users.Any(u => u.PhoneNumber == phoneNumber))
            {
                return new ValidationResult("Phone number is already in use.");
            }

            return ValidationResult.Success;
        }
    }
}
