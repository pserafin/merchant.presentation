using Merchant.CORE.Enums;
using System.Collections.Generic;

namespace Merchant.CORE.EFObjects
{
    public class UserIdentity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<Role> Roles { get; set; }
    }
}
