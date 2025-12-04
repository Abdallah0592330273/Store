using System.ComponentModel.DataAnnotations;

namespace Store.webApi.Dtos.Validation
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date <= DateTime.UtcNow)
                {
                    return new ValidationResult("Date must be in the future.");
                }
            }
            return ValidationResult.Success;
        }
    }

}
