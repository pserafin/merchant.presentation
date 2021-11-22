using System.Collections.Generic;

namespace Merchant.API.ViewModels
{
    public class ValidationResultViewModel
    {
        public bool IsValid { get; set; }

        public IEnumerable<string> Messages { get; set; }
    }
}
