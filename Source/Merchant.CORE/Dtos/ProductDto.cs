namespace Merchant.CORE.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public bool IsEnabled { get; set; }
    }
}
