using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Merchant.CORE.Dtos;
using Merchant.CORE.EFModels;
using Merchant.CORE.IRepositories;
using Merchant.CORE.Models;
using Merchant.CORE.UnitTest;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Merchant.Order.UnitTest.Services.ProductServiceTests
{
    public class ProductServiceFailTests : BaseFixture
    {
        private const int ItemsCount = 3;
        private readonly Exception _dbException = new Exception("DB Error");

        [Fact]
        public async Task Get_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var id = Fixture.Create<int>();
            var enabledOnly = Fixture.Create<bool>();
            Fixture.FreezeMoq<IProductRepository>()
                .Setup(x => x.Get(id, enabledOnly))
                .ThrowsAsync(_dbException);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var exception = await Record.ExceptionAsync(() => productService.Get(id, enabledOnly));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.RetrievalError, "product data"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task List_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var enabledOnly = Fixture.Create<bool>();
            Fixture.FreezeMoq<IProductRepository>()
                .Setup(x => x.List(enabledOnly))
                .ThrowsAsync(_dbException);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var exception = await Record.ExceptionAsync(() => productService.List(enabledOnly));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.RetrievalError, "product list"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task Add_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var model = Fixture.Create<ProductDto>();
            Fixture.FreezeMoq<IProductRepository>()
                .Setup(x => x.Add(It.IsAny<Product>()))
                .ThrowsAsync(_dbException);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var exception = await Record.ExceptionAsync(() => productService.Add(model));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.ModificationError, "add", "new product"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task Update_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var model = Fixture.Create<ProductDto>();
            Fixture.FreezeMoq<IProductRepository>()
                .Setup(x => x.Update(It.IsAny<Product>()))
                .Throws(_dbException);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var exception = await Record.ExceptionAsync(() => productService.Update(model));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.ModificationError, "update", "product"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task UpdateMany_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var model = Fixture.CreateMany<ProductDto>(ItemsCount);
            Fixture.FreezeMoq<IProductRepository>()
                .Setup(x => x.UpdateMany(It.IsAny<IEnumerable<Product>>()))
                .Throws(_dbException);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var exception = await Record.ExceptionAsync(() => productService.UpdateMany(model));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.ModificationError, "update", "product"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task Delete_WhenDbErrorOccured_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var id = Fixture.Create<int>();
            Fixture.FreezeMoq<IProductRepository>()
                .Setup(x => x.Delete(id))
                .Throws(_dbException);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var exception = await Record.ExceptionAsync(() => productService.Delete(id));

            // Assert
            Assert.Equal(string.Format(ErrorMessages.ModificationError, "delete", "product"), ((UIException)exception)?.Message);
        }

        [Fact]
        public async Task Delete_WhenProductIsAddedToOrder_ReturnedUIExceptionHasExpectedMessage()
        {
            // Arrange
            var excetpion = new DbUpdateException("Error", new Exception("The DELETE operation failed"));
            var id = Fixture.Create<int>();
            Fixture.FreezeMoq<IProductRepository>()
                .Setup(x => x.Delete(id))
                .Throws(excetpion);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var exception = await Record.ExceptionAsync(() => productService.Delete(id));

            // Assert
            Assert.Equal(ErrorMessages.ProductDeleteError, ((UIException)exception)?.Message);
        }
    }
}
