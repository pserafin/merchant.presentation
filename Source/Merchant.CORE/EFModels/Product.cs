using System.Collections.Generic;

namespace Merchant.CORE.EFModels
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public bool IsEnabled { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
