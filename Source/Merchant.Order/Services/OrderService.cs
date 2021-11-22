using AutoMapper;
using Merchant.CORE.Dtos;
using Merchant.CORE.Enums;
using Merchant.CORE.IHelpers;
using Merchant.CORE.IRepositories;
using Merchant.CORE.IServices;
using Merchant.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchant.Order.Services
{
    public class OrderService: IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly ICartValidator _cartValidator;

        public OrderService(IMapper mapper, IOrderRepository orderRepository, ICartValidator cartValidator, IProductService productService)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _productService = productService;
            _cartValidator = cartValidator;
        }

        public async Task<OrderDto> Get(int id, int? userId = null)
        {
            try
            {
                var order = await _orderRepository.Get(id, userId);

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.RetrievalError, "order data"), e);
            }
        }

        public async Task<OrderDto> GetCurrent(int userId)
        {
            try
            {
                var order = await _orderRepository.GetCurrent(userId);

                return order != null 
                    ? _mapper.Map<OrderDto>(order)
                    : new OrderDto
                       {
                           Date = DateTime.UtcNow,
                           Items = Enumerable.Empty<OrderItemDto>(),
                           Status = OrderStatus.New,
                           UserId = userId
                       };
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.RetrievalError, "order data"), e);
            }
        }

        public async Task<IEnumerable<OrderDto>> List(int? userId = null)
        {
            try
            {
                var orders = await _orderRepository.List(userId);

                return _mapper.Map<IEnumerable<OrderDto>>(orders);
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.RetrievalError, "order list"), e);
            }
        }

        public async Task<OrderDto> Add(OrderDto orderDto)
        {
            try
            {
                NormalizeModel(orderDto);
                await ValidateOrder(orderDto);

                if (orderDto.Items.Any())
                {
                    return await AddWithItems(orderDto);
                }

                return orderDto;
            }
            catch (UIValidationException)
            {
                throw;
            }
            catch (UIException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.ModificationError, "add", "your order"), e);
            }
        }

        public async Task<OrderDto> Update(OrderDto orderDto)
        {
            try
            {
                NormalizeEditedOrder(orderDto);
                await ValidateOrder(orderDto);

                if (orderDto.Items.Any())
                {
                    var dbOrder = await _orderRepository.Get(orderDto.Id);
                    orderDto.Status = SetStatus(orderDto, dbOrder);

                    if (HaveOrderItemsBeenChanged(orderDto, dbOrder))
                    {
                        return await UpdateModifedOrder(orderDto, dbOrder);
                    }

                    if (HasOrderStatusBeenChanged(orderDto, dbOrder))
                    {
                        await _orderRepository.SetStatus(orderDto.Id, orderDto.Status);
                    }

                    return orderDto;
                }

                return await CleanUpOrder(orderDto);
            }
            catch (UIValidationException)
            {
                throw;
            }
            catch (UIException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.ModificationError, "update", "your order"), e);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await ValidateOrderStatusBeforeDeletion(id);

                var products = await VerifyDeletingOrderWithStorage(id);

                await _orderRepository.Delete(id);
                await _productService.UpdateMany(products);
            }
            catch (UIException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.ModificationError, "delete", "your order"), e);
            }
        }

        public async Task<ValidationResult> Validate(OrderDto orderDto)
        {
            try
            {
                var result = await _cartValidator.ValidateOrder(orderDto);

                if (result.IsValid && orderDto.Status == OrderStatus.New)
                {
                    await _orderRepository.SetStatus(orderDto.Id, OrderStatus.Validated);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new UIException(ErrorMessages.GeneralValidationError, e);
            }
        }

        public async Task<OrderDto> UpdateModifedOrder(OrderDto orderDto, CORE.EFModels.Order dbOrder)
        {
            ValidateOrderStatusBeforeUpdate(orderDto, dbOrder);

            var products = await VerifyOrderWithStorage(orderDto);

            // Please be aware: there is no transaction which would keep both below operations in sync!!
            var order = await _orderRepository.Update(_mapper.Map<CORE.EFModels.Order>(orderDto));
            await _productService.UpdateMany(products);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task SetStatus(int id, OrderStatus status)
        {
            try
            {
                await _orderRepository.SetStatus(id, status);
            }
            catch (Exception e)
            {
                throw new UIException(ErrorMessages.OrderStatusSetError, e);
            }
        }

        private async Task ValidateOrderStatusBeforeDeletion(int id)
        {
            var order = await _orderRepository.Get(id);
            if (order != null && order.Status > OrderStatus.Validated)
            {
                throw new UIException(string.Format(ErrorMessages.OrderModificationForbiden, order.Status.ToString()));
            }
        }

        private OrderStatus SetStatus(OrderDto orderDto, CORE.EFModels.Order order) =>
            orderDto.Status == OrderStatus.New && order.Status == OrderStatus.New
                ? OrderStatus.Validated
                : orderDto.Status;

        private bool HaveOrderItemsBeenChanged(OrderDto orderDto, CORE.EFModels.Order order) =>
            order.Items.Count != orderDto.Items.Count()
            || !order.Items.All(x => orderDto.Items.Any(y => y.ProductId == x.ProductId && y.Quantity == x.Quantity))
            || !orderDto.Items.All(x => order.Items.Any(y => y.ProductId == x.ProductId && y.Quantity == x.Quantity));

        private bool HasOrderStatusBeenChanged(OrderDto orderDto, CORE.EFModels.Order order) =>
            order.Status != orderDto.Status;

        private void ValidateOrderStatusBeforeUpdate(OrderDto orderDto, CORE.EFModels.Order order)
        {
            if (orderDto.Status > OrderStatus.Validated &&
                (order.Items.Count != orderDto.Items.Count()
                 || !order.Items.All(x => orderDto.Items.Any(y => y.ProductId == x.ProductId && y.Quantity == x.Quantity))
                 || !orderDto.Items.All(x => order.Items.Any(y => y.ProductId == x.ProductId && y.Quantity == x.Quantity))))
            {
                throw new UIException(string.Format(ErrorMessages.OrderModificationForbiden, order.Status.ToString()));
            }
        }

        private async Task ValidateOrder(OrderDto orderDto)
        {
            var validation = await _cartValidator.ValidateOrder(orderDto);
            if (!validation.IsValid)
            {
                var builder = new StringBuilder("");
                foreach (var error in validation.Messages)
                {
                    builder.AppendLine(error);
                }

                throw new UIValidationException(builder.ToString());
            }
        }

        private async Task<OrderDto> AddWithItems(OrderDto orderDto)
        {
            var products = await VerifyNewOrderWithStorage(orderDto);

            // Please be aware: there is no transaction which would keep both below operations in sync!!
            var order = await _orderRepository.Add(_mapper.Map<CORE.EFModels.Order>(orderDto));
            await _productService.UpdateMany(products);

            return _mapper.Map<OrderDto>(order);
        }

        private void NormalizeModel(OrderDto order)
        {
            order.Id = 0;
            order.Status = OrderStatus.New;
            foreach (var item in order.Items)
            {
                item.Id = 0;
            }
            order.TotalPrice = _cartValidator.CalculateTotal(order);
            order.Items = order.Items.Where(x => x.Quantity > 0).ToList();
        }

        private void NormalizeEditedOrder(OrderDto order)
        {
            order.TotalPrice = _cartValidator.CalculateTotal(order);
            order.Items = order.Items.Where(x => x.Quantity > 0).ToList();
        }

        private async Task<OrderDto> CleanUpOrder(OrderDto orderDto)
        {
            await Delete(orderDto.Id);

            return new OrderDto {UserId = orderDto.UserId};
        }

        private async Task<IEnumerable<ProductDto>> VerifyNewOrderWithStorage(OrderDto orderDto)
        {
            var products = await _productService.List(orderDto.Items.Select(x => x.ProductId));

            foreach (var product in products)
            {
                var orderItem = orderDto.Items.First(x => x.ProductId == product.Id);
                product.Quantity -= orderItem.Quantity;
            }

            return products;
        }

        private async Task<IEnumerable<ProductDto>> VerifyOrderWithStorage(OrderDto orderDto)
        {
            var dbOrder = await Get(orderDto.Id);
            var productIds = dbOrder.Items.Select(x => x.ProductId).ToList();
            productIds.AddRange(orderDto.Items.Select(x => x.ProductId));
            
            var products = await _productService.List(productIds.Distinct());

            foreach (var product in products)
            {
                var dbOrderItem = dbOrder.Items.FirstOrDefault(x => x.ProductId == product.Id);
                var orderItem = orderDto.Items.FirstOrDefault(x => x.ProductId == product.Id);

                if (dbOrderItem == null)
                {
                    product.Quantity -= orderItem?.Quantity ?? 0;
                } 
                else if (orderItem == null)
                {
                    product.Quantity += dbOrderItem.Quantity;
                }
                else
                {
                    product.Quantity += dbOrderItem.Quantity - orderItem.Quantity;
                }
                
            }

            return products;
        }

        private async Task<IEnumerable<ProductDto>> VerifyDeletingOrderWithStorage(int id)
        {
            var dbOrder = await Get(id);
            var products = await _productService.List(dbOrder.Items.Select(x => x.ProductId));

            foreach (var product in products)
            {
                var dbOrderItem = dbOrder.Items.FirstOrDefault(x => x.ProductId == product.Id);
                product.Quantity += dbOrderItem?.Quantity ?? 0;
            }

            return products;
        }
    }
}
