using System.Collections.Generic;

namespace Merchant.CORE.EFModels
{
    public class OrderUser
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
