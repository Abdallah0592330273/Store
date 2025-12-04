using System.ComponentModel.DataAnnotations;

namespace Store.webApi.Dtos.Validation
{
    public class ValidEnumAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public ValidEnumAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (!Enum.IsDefined(_enumType, value))
            {
                return new ValidationResult($"Invalid value for {validationContext.DisplayName}. Valid values are: {string.Join(", ", Enum.GetNames(_enumType))}");
            }

            return ValidationResult.Success;
        }
    }
}
