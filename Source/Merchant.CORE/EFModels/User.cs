using Microsoft.AspNetCore.Identity;

namespace Merchant.CORE.EFModels
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }
    }
}
