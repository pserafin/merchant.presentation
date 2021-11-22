using System;
using System.Threading.Tasks;
using AutoFixture;
using Merchant.CORE.Dtos;
using Merchant.CORE.Enums;
using Merchant.CORE.IHelpers;
using Merchant.CORE.IRepositories;
using Merchant.CORE.Models;
using Merchant.CORE.UnitTest;
using Moq;
using Xunit;

namespace Merchant.Order.UnitTest.Services.OrderServiceTests
{
    public class OrderServiceFailTests : BaseFixture
    {
        private const string Error = "DB Error";
        private readonly Exception _dbException = new Exception(Error);

        [Fact]
        public async Task Get_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var orderId = Fixture.Create<int>();
            var userId = Fixture.Create<int?>();
            Fixture.FreezeMoq<IOrderRepository>()
                .Setup(x => x.Get(orderId, userId))
                .ThrowsAsync(_dbException);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.Get(orderId, userId));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.RetrievalError, "order data"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task GetCurrent_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var userId = Fixture.Create<int>();
            Fixture.FreezeMoq<IOrderRepository>()
                .Setup(x => x.GetCurrent(userId))
                .ThrowsAsync(_dbException);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.GetCurrent(userId));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.RetrievalError, "order data"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task List_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var userId = Fixture.Create<int?>();
            Fixture.FreezeMoq<IOrderRepository>()
                .Setup(x => x.List(userId))
                .ThrowsAsync(_dbException);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.List(userId));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.RetrievalError, "order list"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task Add_WhenNullIsPassedAsInput_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.Add(null));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.ModificationError, "add", "your order"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task Add_WhenOrderDoesNotPassValidation_ReturnedUIValidationExceptionHasExpectedMessage()
        {
            // Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Status, OrderStatus.Resigned)
                .Create();
            Fixture.Inject(ValidationResult.Invalid(new [] { Error }));
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.Add(model));

            // Assert
            Assert.Equal($"{Error}\r\n", ((UIValidationException)exception)?.Message);
        }
        
        [Fact]
        public async Task Add_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var model = Fixture.Create<OrderDto>();
            Fixture.FreezeMoq<IOrderRepository>()
                .Setup(x => x.Add(It.IsAny<CORE.EFModels.Order>()))
                .ThrowsAsync(_dbException);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.Add(model));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.ModificationError, "add", "your order"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task Update_WhenNullIsPassedAsInput_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.Update(null));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.ModificationError, "update", "your order"), ((UIException)exception)?.Message);
        }


        [Fact]
        public async Task Update_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var model = Fixture.Create<OrderDto>();
            Fixture.FreezeMoq<IOrderRepository>()
                .Setup(x => x.Update(It.IsAny<CORE.EFModels.Order>()))
                .ThrowsAsync(_dbException);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.Update(model));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.ModificationError, "update", "your order"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task Update_WhenOrderDoesNotPassValidation_ReturnedUIValidationExceptionHasExpectedMessage()
        {
            // Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Status, OrderStatus.Resigned)
                .Create();
            Fixture.Inject(ValidationResult.Invalid(new[] { Error }));
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.Update(model));

            // Assert
            Assert.Equal($"{Error}\r\n", ((UIValidationException)exception)?.Message);
        }

        [Fact]
        public async Task Delete_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var orderId = Fixture.Create<int>();
            Fixture.FreezeMoq<IOrderRepository>()
                .Setup(x => x.Delete(orderId))
                .Throws(_dbException);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.Delete(orderId));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.ModificationError, "delete", "your order"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task Validate_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var model = Fixture.Create<OrderDto>();
            Fixture.FreezeMoq<ICartValidator>()
                .Setup(x => x.ValidateOrder(model))
                .ThrowsAsync(_dbException);
            var orderService = Fixture.Create<Order.Services.OrderService>();

            // Act
            var exception = await Record.ExceptionAsync(() => orderService.Validate(model));

            // Assert
            Assert.Equal(ErrorMessages.GeneralValidationError, ((UIException)exception)?.Message);
        }

    }
}
