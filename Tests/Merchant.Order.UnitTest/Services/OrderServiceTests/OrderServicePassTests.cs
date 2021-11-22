using System.Collections.Generic;
using AutoFixture;
using AutoMapper;
using Merchant.API.Setup;
using Merchant.CORE.Dtos;
using Merchant.CORE.Enums;
using Merchant.CORE.IHelpers;
using Merchant.CORE.IRepositories;
using Merchant.CORE.Models;
using Merchant.CORE.UnitTest;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Merchant.Order.UnitTest.Services.OrderServiceTests
{
    public class OrderServicePassTests : BaseFixture
    {
        private const int ExpectedItemsCount = 3;
        private const decimal ExpectedTotalPrice = 10.1m;
        private const decimal FirstProductPrice = 1.1m;
        private const int FirstProductQuantity = 5;
        private const decimal SecondProductPrice = 2.3m;
        private const int SecondProductQuantity = 2;
        private const int ExpectedNewOrderId = 0;
        private const int Id = 1;
        private const int Quantity = 1;

        private readonly IMapper _mapper;

        public OrderServicePassTests()
        {
            var config = new MapperConfiguration(opts => opts.AddProfile(new AutoMapperProfile()));
            _mapper = config.CreateMapper();
            Fixture.Inject(_mapper);
        }

        [Fact]
        public async Task Get_WhenOrderWithSpecifiedIdExists_ReturnsModel()
        {
            // Arrange
            var orderId = Fixture.Create<int>();
            var userId = Fixture.Create<int?>();
            var efModel = Fixture.Create<CORE.EFModels.Order>();
            Fixture.Inject(efModel);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var result = await orderService.Get(orderId, userId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCurrent_WhenOpenOrderExists_ReturnsModel()
        {
            // Arrange
            var userId = Fixture.Create<int>();
            var efModel = Fixture.Create<CORE.EFModels.Order>();
            Fixture.Inject(efModel);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var result = await orderService.GetCurrent(userId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCurrent_WhenOpenOrderDoesNotExist_ReturnsNewModelWithZeroId()
        {
            // Arrange
            var userId = Fixture.Create<int>();
            Fixture.Inject<CORE.EFModels.Order>(null);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var result = await orderService.GetCurrent(userId);

            // Assert
            Assert.Equal(ExpectedNewOrderId, result.Id);
        }

        [Fact]
        public async Task List_WhenOrdersMatchingInputCriteriaExist_ReturnsAllOrders()
        {
            // Arrange
            var userId = Fixture.Create<int?>();
            var efModel = Fixture.CreateMany<CORE.EFModels.Order>(ExpectedItemsCount);
            Fixture.Inject(efModel);
            Fixture.Inject(ValidationResult.Valid());
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var result = await orderService.List(userId);

            // Assert
            Assert.Equal(ExpectedItemsCount, result.Count());
        }

        [Fact]
        public async Task Add_WhenCalledForValidOrder_OrderIsBeingPersistedWithNewStatus()
        {
            // Arrange
            var model = CreateOrder();
            Fixture.Inject(CreateProducts());
            Fixture.Inject(ValidationResult.Valid());
            var orderRepositoryMoq = Fixture.FreezeMoq<IOrderRepository>();
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            await orderService.Add(model);

            // Assert
            orderRepositoryMoq.Verify(x => x.Add(It.Is<CORE.EFModels.Order>(p => p.Status == OrderStatus.New)), Times.Once);
        }

        [Fact]
        public async Task Add_WhenCalledForValidOrderWithoutItems_OrderIsNotBeingPersisted()
        {
            // Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Status, OrderStatus.Resigned)
                .With(x => x.Items, Enumerable.Empty<OrderItemDto>())
                .Create();
            Fixture.Inject(ValidationResult.Valid());
            var orderRepositoryMoq = Fixture.FreezeMoq<IOrderRepository>();
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            await orderService.Add(model);

            // Assert
            orderRepositoryMoq.Verify(x => x.Add(It.IsAny<CORE.EFModels.Order>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenCalledForValidOrder_OrderIsBeingPersistedWithCorrecltyRecalculatedTotalPrice()
        {
            // Arrange
            var orderItems = new[]
            {
                Fixture.Build<OrderItemDto>()
                    .With(x => x.Price, FirstProductPrice)
                    .With(x => x.Quantity, FirstProductQuantity)
                    .With(x => x.ProductId, Id)
                    .Create(),
                Fixture.Build<OrderItemDto>()
                    .With(x => x.Price, SecondProductPrice)
                    .With(x => x.Quantity, SecondProductQuantity)
                    .Create()
            };
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Status, OrderStatus.New)
                .With(x => x.Items, orderItems)
                .With(x => x.TotalPrice, 0.0m)
                .Create();
            var dbModel = _mapper.Map<CORE.EFModels.Order>(model);
            dbModel.Items.First().Quantity = SecondProductQuantity;
            Fixture.Inject(CreateProducts());
            var carValidatorMoq = Fixture.FreezeMoq<ICartValidator>();
            carValidatorMoq.Setup(x => x.CalculateTotal(It.IsAny<OrderDto>()))
                .Returns(ExpectedTotalPrice);
            carValidatorMoq.Setup(x => x.ValidateOrder(It.IsAny<OrderDto>()))
                .ReturnsAsync(ValidationResult.Valid());
            var orderRepositoryMoq = Fixture.FreezeMoq<IOrderRepository>();
            orderRepositoryMoq.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(dbModel);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            await orderService.Update(model);

            // Assert
            orderRepositoryMoq.Verify(x => x.Update(It.Is<CORE.EFModels.Order>(p => p.TotalPrice == ExpectedTotalPrice)), Times.Once);
        }

        [Fact]
        public async Task Update_WhenCalledForOrderWithNoProducts_OrderIsBeingRemoved()
        {
            // Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Status, OrderStatus.Validated)
                .With(x => x.Items, Enumerable.Empty<OrderItemDto>())
                .Create();
            Fixture.Inject(CreateProducts());
            var cartValidatorMoq = Fixture.FreezeMoq<ICartValidator>();
            cartValidatorMoq.Setup(x => x.CalculateTotal(It.IsAny<OrderDto>()))
                .Returns(ExpectedTotalPrice);
            cartValidatorMoq.Setup(x => x.ValidateOrder(It.IsAny<OrderDto>()))
                .ReturnsAsync(ValidationResult.Valid());
            var orderRepositoryMoq = Fixture.FreezeMoq<IOrderRepository>();
            orderRepositoryMoq.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(_mapper.Map<CORE.EFModels.Order>(model));
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            await orderService.Update(model);

            // Assert
            orderRepositoryMoq.Verify(x => x.Delete(model.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenCalled_OrderIsBeingDeleted()
        {
            // Arrange
            var orderItems = new[]
            {
                Fixture.Build<OrderItemDto>()
                    .With(x => x.Price, FirstProductPrice)
                    .With(x => x.Quantity, FirstProductQuantity)
                    .With(x => x.ProductId, Id)
                    .Create(),
                Fixture.Build<OrderItemDto>()
                    .With(x => x.Price, SecondProductPrice)
                    .With(x => x.Quantity, SecondProductQuantity)
                    .Create()
            };
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Status, OrderStatus.Validated)
                .With(x => x.Items, orderItems)
                .With(x => x.TotalPrice, 0.0m)
                .Create();
            Fixture.Inject(CreateProducts());
            var orderRepositoryMoq = Fixture.FreezeMoq<IOrderRepository>();
            orderRepositoryMoq.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(_mapper.Map<CORE.EFModels.Order>(model));
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            await orderService.Delete(model.Id);

            // Assert
            orderRepositoryMoq.Verify(x => x.Delete(model.Id), Times.Once);
        }

        [Fact]
        public async Task Validate_WhenCalled_OrderIsBeingValidated()
        {
            // Arrange
            var order = Fixture.Create<OrderDto>();
            var cartValidatorMoq = Fixture.FreezeMoq<ICartValidator>();
            cartValidatorMoq.Setup(x => x.ValidateOrder(It.IsAny<OrderDto>()))
                .ReturnsAsync(ValidationResult.Valid());
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            await orderService.Validate(order);

            // Assert
            cartValidatorMoq.Verify(x => x.ValidateOrder(order), Times.Once);
        }

        [Fact]
        public async Task Validate_WhenCalledForNewOrder_OrderStatusIsSetAsValidated()
        {
            // Arrange
            var order = Fixture.Build<OrderDto>()
                .With(x => x.Status, OrderStatus.New)
                .Create();
            Fixture.FreezeMoq<ICartValidator>()
                .Setup(x => x.ValidateOrder(It.IsAny<OrderDto>()))
                .ReturnsAsync(ValidationResult.Valid());
            var orderRepositoryMoq = Fixture.FreezeMoq<IOrderRepository>();
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            await orderService.Validate(order);

            // Assert
            orderRepositoryMoq.Verify(x => x.SetStatus(order.Id, OrderStatus.Validated), Times.Once);
        }

        private OrderDto CreateOrder()
        {
            var orderItem = Fixture.Build<OrderItemDto>()
                .With(x => x.ProductId, Id)
                .With(x => x.Quantity, Quantity)
                .Create();

            return Fixture.Build<OrderDto>()
                .With(x => x.Items, new[] { orderItem })
                .With(x => x.Status, OrderStatus.Resigned)
                .Create();
        }

        private IEnumerable<ProductDto> CreateProducts()
        {
            var product = Fixture.Build<ProductDto>()
                .With(x => x.Id, Id)
                .With(x => x.Quantity, Quantity)
                .Create();

            return new[] { product };
        }

    }
}
