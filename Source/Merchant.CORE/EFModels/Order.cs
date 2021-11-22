using System;
using System.Collections.Generic;
using Merchant.CORE.Enums;

namespace Merchant.CORE.EFModels
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }

        public virtual OrderUser User { get; set; }
    }
}
