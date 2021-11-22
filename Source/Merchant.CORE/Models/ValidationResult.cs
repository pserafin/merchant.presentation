using System.Collections.Generic;

namespace Merchant.CORE.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public static ValidationResult Valid() =>
            new ValidationResult
            {
                IsValid = true
            };

        public static ValidationResult Invalid(IEnumerable<string> errors) =>
            new ValidationResult
            {
                IsValid = false,
                Messages = errors
            };
    }
}
