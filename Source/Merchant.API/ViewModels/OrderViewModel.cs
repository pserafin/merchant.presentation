using System;
using System.Collections.Generic;
using Merchant.CORE.Enums;

namespace Merchant.API.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserAddress { get; set; }

        public string UserEmail { get; set; }

        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public IEnumerable<OrderItemViewModel> Items { get; set; }
    }
}
