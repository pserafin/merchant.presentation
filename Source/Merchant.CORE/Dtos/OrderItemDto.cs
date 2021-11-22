namespace Merchant.CORE.Dtos
{
    public class OrderItemDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
