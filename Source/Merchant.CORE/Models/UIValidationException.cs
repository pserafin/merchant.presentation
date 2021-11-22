using System;

namespace Merchant.CORE.Models
{
    public class UIValidationException : Exception
    {
        public UIValidationException(string message) : base(message) { }

        public UIValidationException(string message, Exception exception) : base(message, exception) { }
    }
}
