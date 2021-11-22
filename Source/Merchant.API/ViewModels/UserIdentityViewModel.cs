using System.Collections.Generic;

namespace Merchant.API.ViewModels
{
    public class UserIdentityViewModel
    {
        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<string> Roles { get; set; }
    
    }
}
