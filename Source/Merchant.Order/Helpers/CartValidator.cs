using Merchant.CORE.Dtos;
using Merchant.CORE.EFModels;
using Merchant.CORE.IHelpers;
using Merchant.CORE.IRepositories;
using Merchant.CORE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Merchant.CORE.Enums;
using Newtonsoft.Json;

namespace Merchant.Order.Helpers
{
    public class CartValidator :ICartValidator
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly JsonSerializerSettings _serializerSettings;

        public CartValidator(IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _serializerSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
        }

        public async Task<ValidationResult> ValidateOrder(OrderDto order)
        {
            if (order?.Items == null || !order.Items.Any())
            {
                return ValidationResult.Valid();
            }

            return (order.Id == 0)
                ? await ValidateNewOrder(order)
                : await ValidateExistingOrder(order);
        }

        public decimal CalculateTotal(OrderDto order) =>
            (order?.Items == null || !order.Items.Any())
                ? 0.0m
                : order.Items.Sum(x => x.Price * x.Quantity);


        private async Task<ValidationResult> ValidateNewOrder(OrderDto order)
        {
            var validationMessages = new List<string>();
            var products = await GetProducts(order);

            var availability = ValidateProductsAvailability(order, products);
            var prices = ValidateProductsPrices(order, products, OrderStatus.New);

            validationMessages.AddRange(availability.Select(x => x.Value));
            validationMessages.AddRange(prices.Where(x => availability.All(y => y.Key != x.Key)).Select(z => z.Value));

            return validationMessages.Any()
                ? ValidationResult.Invalid(validationMessages)
                : ValidationResult.Valid();
        }

        private async Task<ValidationResult> ValidateExistingOrder(OrderDto order)
        {
            var validationMessages = new List<string>();
            var products = await GetProducts(order);
            var sourceOrder = await _orderRepository.Get(order.Id);
            var orderDiff = CreateDifferentialModel(order, sourceOrder);

            var availability = ValidateProductsAvailability(orderDiff, products);
            var prices = ValidateProductsPrices(orderDiff, products, sourceOrder.Status);

            validationMessages.AddRange(availability.Select(x => x.Value));
            validationMessages.AddRange(prices.Where(x => availability.All(y => y.Key != x.Key)).Select(z => z.Value));

            return validationMessages.Any()
                ? ValidationResult.Invalid(validationMessages)
                : ValidationResult.Valid();
        }

        private OrderDto CreateDifferentialModel(OrderDto order, CORE.EFModels.Order sourceOrder)
        {
            var orderCopy = DeepClone(order);

            foreach (var item in orderCopy.Items)
            {
                var sourceItem = sourceOrder.Items.FirstOrDefault(x => x.ProductId == item.ProductId);
                if (sourceItem != null)
                {
                    var difference = item.Quantity - sourceItem.Quantity;
                    item.Quantity = (difference > 0) ? difference : 0;
                }
            }

            orderCopy.Items = orderCopy.Items.Where(x => x.Quantity > 0).ToList();

            return orderCopy;
        }

        private OrderDto DeepClone(OrderDto order) =>
            JsonConvert.DeserializeObject<OrderDto>(JsonConvert.SerializeObject(order), _serializerSettings);

        private IDictionary<OrderItemDto, string> ValidateProductsPrices(OrderDto order, IEnumerable<Product> products, OrderStatus status)
        {
            var validationMessages = new Dictionary<OrderItemDto, string>();
            if (status > OrderStatus.Validated)
            {
                return validationMessages;
            }

            foreach (var item in order.Items)
            {
                var product = products.FirstOrDefault(x => x.Id == item.ProductId);

                if (product != null && item.Price != product.Price)
                {
                    validationMessages.Add(item, string.Format(ErrorMessages.PriceChangedError, item.Name, product.Price));
                }
            }
            
            return validationMessages;
        }

        private IDictionary<OrderItemDto, string> ValidateProductsAvailability(OrderDto order, IEnumerable<Product> products)
        {
            var validationMessages = new Dictionary<OrderItemDto, string>();

            foreach (var item in order.Items)
            {
                var product = products.FirstOrDefault(x => x.Id == item.ProductId);

                if (product == null)
                {
                    validationMessages.Add(item, string.Format(ErrorMessages.ProductNoLongerAvailableError, item.Name));
                }
                else if (!product.IsEnabled && product.Id > 0)
                {
                    validationMessages.Add(item, string.Format(ErrorMessages.ProductTemporarilyAvailableError, item.Name));
                }
                else if (item.Quantity > product.Quantity || product.Quantity < 1)
                {
                    validationMessages.Add(item,
                        string.Format(ErrorMessages.NotEnoughProductsError, item.Name));
                }
            }

            return validationMessages;
        }

        private async Task<IEnumerable<Product>> GetProducts(OrderDto order) =>
            (order.Items == null || !order.Items.Any())
                ? Enumerable.Empty<Product>()
                : await _productRepository.List(order.Items.Select(x => x.ProductId));
    }
}
