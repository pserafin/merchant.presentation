using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Merchant.CORE.Dtos;
using Merchant.CORE.EFModels;
using Merchant.CORE.Models;
using Merchant.CORE.UnitTest;
using Merchant.Order.Helpers;
using Xunit;

namespace Merchant.Order.UnitTest.Helpers.CartValidatorTests
{
    public class CartValidatorPassTests : BaseFixture
    {
        private const int NewOrderId = 0;
        private const decimal ZeroAmount = 0.0m;
        private const decimal ExpectedAmount = 10.1m;
        private const decimal FirstProductId = 1;
        private const decimal FirstProductPrice = 1.1m;
        private const int FirstProductQuantity = 5;
        private const decimal SecondProductId = 2;
        private const decimal SecondProductPrice = 2.3m;
        private const int SecondProductQuantity = 2;
        private const int LowQuantity = 1;

        [Theory]
        [ClassData(typeof(EmptyModelOrModelItemsTheoryData))]
        public void CalculateTotal_WhenModelIsEmptyOrHasNoItems_ReturnsZero(OrderDto theoryData)
        {
            //Arrange
            var cartValidator = Fixture.Create<CartValidator>();

            //Act
            var totalAmount = cartValidator.CalculateTotal(theoryData);

            //Assert
            Assert.Equal(ZeroAmount,  totalAmount);
        }

        [Fact]
        public void CalculateTotal_WhenModelHasItems_ReturnsExpectedAmount()
        {
            //Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Id, NewOrderId)
                .With(x => x.Items, GetBasicOrderDtoItems())
                .With(x => x.TotalPrice, ZeroAmount)
                .Create();
            var cartValidator = Fixture.Create<CartValidator>();

            //Act
            var totalAmount = cartValidator.CalculateTotal(model);

            //Assert
            Assert.Equal(ExpectedAmount, totalAmount);
        }

        [Theory]
        [ClassData(typeof(EmptyModelOrModelItemsTheoryData))]
        public async Task ValidateOrder_WhenModelIsEmptyOrHasNoItems_ReturnsValidResult(OrderDto theoryData)
        {
            //Arrange
            var cartValidator = Fixture.Create<CartValidator>();

            //Act
            var validationModel = await cartValidator.ValidateOrder(theoryData);

            //Assert
            Assert.True(validationModel.IsValid);
        }

        [Fact]
        public async Task ValidateOrder_WhenOrderContainsEnabledProductsWithUnchangedPrices_ReturnsValidResult()
        {
            //Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Id, NewOrderId)
                .With(x => x.Items, GetBasicOrderDtoItems())
                .With(x => x.TotalPrice, ExpectedAmount)
                .Create();
            Fixture.Inject(GetBasicProducts());
            var cartValidator = Fixture.Create<CartValidator>();

            //Act
            var validationModel = await cartValidator.ValidateOrder(model);

            //Assert
            Assert.True(validationModel.IsValid);
        }

        [Fact]
        public async Task ValidateOrder_WhenOrderContainsDisabledProduct_ReturnsExpectedErrorMessage()
        {
            //Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Id, NewOrderId)
                .With(x => x.Items, GetBasicOrderDtoItems())
                .With(x => x.TotalPrice, ExpectedAmount)
                .Create();
            Fixture.Inject(GetDisabledProducts());
            var cartValidator = Fixture.Create<CartValidator>();

            //Act
            var validationModel = await cartValidator.ValidateOrder(model);

            //Assert
            Assert.NotEmpty(validationModel.Messages.Where(x => 
                x == string.Format(ErrorMessages.ProductTemporarilyAvailableError, nameof(FirstProductId))));
        }

        [Fact]
        public async Task ValidateOrder_WhenOrderContainsRemovedProduct_ReturnsExpectedErrorMessage()
        {
            //Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Id, NewOrderId)
                .With(x => x.Items, GetBasicOrderDtoItems())
                .With(x => x.TotalPrice, ExpectedAmount)
                .Create();
            Fixture.Inject(Enumerable.Empty<Product>());
            var cartValidator = Fixture.Create<CartValidator>();

            //Act
            var validationModel = await cartValidator.ValidateOrder(model);

            //Assert
            Assert.NotEmpty(validationModel.Messages.Where(x =>
                x == string.Format(ErrorMessages.ProductNoLongerAvailableError, nameof(FirstProductId))));
        }

        [Fact]
        public async Task ValidateOrder_WhenOrderContainsMoreProductThanIsAvailable_ReturnsExpectedErrorMessage()
        {
            //Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Id, NewOrderId)
                .With(x => x.Items, GetBasicOrderDtoItems())
                .With(x => x.TotalPrice, ExpectedAmount)
                .Create();
            Fixture.Inject(GetLowQuantityProducts());
            var cartValidator = Fixture.Create<CartValidator>();

            //Act
            var validationModel = await cartValidator.ValidateOrder(model);

            //Assert
            Assert.NotEmpty(validationModel.Messages.Where(x =>
                x == string.Format(ErrorMessages.NotEnoughProductsError, nameof(FirstProductId))));
        }

        [Fact]
        public async Task ValidateOrder_WhenCartProductsHaveNotActualPrices_ReturnsExpectedErrorMessage()
        {
            //Arrange
            var model = Fixture.Build<OrderDto>()
                .With(x => x.Id, NewOrderId)
                .With(x => x.Items, GetBasicOrderDtoItems())
                .With(x => x.TotalPrice, ExpectedAmount)
                .Create();
            Fixture.Inject(GetChangedPriceProducts());
            var cartValidator = Fixture.Create<CartValidator>();

            //Act
            var validationModel = await cartValidator.ValidateOrder(model);

            //Assert
            Assert.NotEmpty(validationModel.Messages.Where(x =>
                x == string.Format(ErrorMessages.PriceChangedError, nameof(FirstProductId), SecondProductPrice)));
        }

        private IEnumerable<OrderItemDto> GetBasicOrderDtoItems() =>
            new[]
            {
                Fixture.Build<OrderItemDto>()
                    .With(x => x.ProductId, FirstProductId)
                    .With(x => x.Price, FirstProductPrice)
                    .With(x => x.Name, nameof(FirstProductId))
                    .With(x => x.Quantity, FirstProductQuantity)
                    .Create(),
                Fixture.Build<OrderItemDto>()
                    .With(x => x.ProductId, SecondProductId)
                    .With(x => x.Price, SecondProductPrice)
                    .With(x => x.Name, nameof(SecondProductId))
                    .With(x => x.Quantity, SecondProductQuantity)
                    .Create()
            };

        private IEnumerable<Product> GetBasicProducts() =>
            new[]
            {
                Fixture.Build<Product>()
                    .With(x => x.Id, FirstProductId)
                    .With(x => x.Price, FirstProductPrice)
                    .With(x => x.Quantity, FirstProductQuantity + 100)
                    .With(x => x.Name, nameof(FirstProductId))
                    .With(x => x.IsEnabled, true)
                    .Create(),
                Fixture.Build<Product>()
                    .With(x => x.Id, SecondProductId)
                    .With(x => x.Price, SecondProductPrice)
                    .With(x => x.Quantity, SecondProductQuantity + 100)
                    .With(x => x.Name, nameof(SecondProductId))
                    .With(x => x.IsEnabled, true)
                    .Create()
            };

        private IEnumerable<Product> GetChangedPriceProducts() =>
            new[]
            {
                Fixture.Build<Product>()
                    .With(x => x.Id, FirstProductId)
                    .With(x => x.Price, SecondProductPrice)
                    .With(x => x.Quantity, FirstProductQuantity + 100)
                    .With(x => x.Name, nameof(FirstProductId))
                    .With(x => x.IsEnabled, true)
                    .Create(),
                Fixture.Build<Product>()
                    .With(x => x.Id, SecondProductId)
                    .With(x => x.Price, FirstProductPrice)
                    .With(x => x.Quantity, SecondProductQuantity + 100)
                    .With(x => x.Name, nameof(SecondProductId))
                    .With(x => x.IsEnabled, true)
                    .Create()
            };

        private IEnumerable<Product> GetDisabledProducts() =>
            new[]
            {
                Fixture.Build<Product>()
                    .With(x => x.Id, FirstProductId)
                    .With(x => x.Price, FirstProductPrice)
                    .With(x => x.Quantity, 1)
                    .With(x => x.Name, nameof(FirstProductId))
                    .With(x => x.IsEnabled, false)
                    .Create(),
                Fixture.Build<Product>()
                    .With(x => x.Id, SecondProductId)
                    .With(x => x.Price, SecondProductPrice)
                    .With(x => x.Quantity, SecondProductQuantity + 100)
                    .With(x => x.Name, nameof(SecondProductId))
                    .With(x => x.IsEnabled, false)
                    .Create()
            };

        private IEnumerable<Product> GetLowQuantityProducts() =>
            new[]
            {
                Fixture.Build<Product>()
                    .With(x => x.Id, FirstProductId)
                    .With(x => x.Price, FirstProductPrice)
                    .With(x => x.Quantity, LowQuantity)
                    .With(x => x.Name, nameof(FirstProductId))
                    .With(x => x.IsEnabled, true)
                    .Create(),
                Fixture.Build<Product>()
                    .With(x => x.Id, SecondProductId)
                    .With(x => x.Price, SecondProductPrice)
                    .With(x => x.Quantity, LowQuantity)
                    .With(x => x.Name, nameof(SecondProductId))
                    .With(x => x.IsEnabled, true)
                    .Create()
            };
    }
}
