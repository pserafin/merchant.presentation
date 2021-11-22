using System;

namespace Merchant.CORE.Models
{
    public class UIException : Exception
    {
        public UIException(string message) : base(message) {}

        public UIException(string message, Exception exception) : base(message, exception) { }
    }
}
