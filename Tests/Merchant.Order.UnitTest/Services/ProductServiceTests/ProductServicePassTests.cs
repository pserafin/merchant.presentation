using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Merchant.API.Setup;
using Merchant.CORE.Dtos;
using Merchant.CORE.EFModels;
using Merchant.CORE.IRepositories;
using Merchant.CORE.UnitTest;
using Moq;
using Xunit;

namespace Merchant.Order.UnitTest.Services.ProductServiceTests
{
    public class ProductServicePassTests : BaseFixture
    {
        private const int ExpectedItemsCount = 3;

        public ProductServicePassTests()
        {
            var config = new MapperConfiguration(opts => opts.AddProfile(new AutoMapperProfile()));
            Fixture.Inject(config.CreateMapper());
        }

        [Fact]
        public async Task Get_WhenProductWithSpecifiedIdExists_ReturnsModel()
        {
            // Arrange
            var id = Fixture.Create<int>();
            var enabledOnly = Fixture.Create<bool>();
            var efModel = Fixture.Create<Product>();
            Fixture.Inject(efModel);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var result = await productService.Get(id, enabledOnly);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task List_WhenProductsMatchingInputCriteriaExist_ReturnsAllProducts()
        {
            // Arrange
            var enabledOnly = Fixture.Create<bool>();
            var efModel = Fixture.CreateMany<Product>(ExpectedItemsCount);
            Fixture.Inject(efModel);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var result = await productService.List(enabledOnly);

            // Assert
            Assert.Equal(ExpectedItemsCount, result.Count());
        }

        [Fact]
        public async Task List_WhenProductsMatchingIdsExists_ReturnsAllProducts()
        {
            // Arrange
            var ids = Fixture.CreateMany<int>(ExpectedItemsCount);
            var efModel = Fixture.CreateMany<Product>(ExpectedItemsCount);
            Fixture.Inject(efModel);
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            var result = await productService.List(ids);

            // Assert
            Assert.Equal(ExpectedItemsCount, result.Count());
        }

        [Fact]
        public async Task Add_WhenCalled_ProductIsBeingPersisted()
        {
            // Arrange
            var model = Fixture.Create<ProductDto>();
            var productRepositoryMoq = Fixture.FreezeMoq<IProductRepository>();
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            await productService.Add(model);

            // Assert
            productRepositoryMoq.Verify(x => x.Add(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenCalled_ProductIsBeingPersisted()
        {
            // Arrange
            var model = Fixture.Create<ProductDto>();
            var productRepositoryMoq = Fixture.FreezeMoq<IProductRepository>();
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            await productService.Update(model);

            // Assert
            productRepositoryMoq.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateMany_WhenCalled_ProductIsBeingPersisted()
        {
            // Arrange
            var model = Fixture.CreateMany<ProductDto>(ExpectedItemsCount);
            var productRepositoryMoq = Fixture.FreezeMoq<IProductRepository>();
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            await productService.UpdateMany(model);

            // Assert
            productRepositoryMoq.Verify(x => x.UpdateMany(It.IsAny<IEnumerable<Product>>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenCalled_ProductIsBeingDeleted()
        {
            // Arrange
            var id = Fixture.Create<int>();
            var productRepositoryMoq = Fixture.FreezeMoq<IProductRepository>();
            var productService = Fixture.Create<Order.Services.ProductService>();

            // Act
            await productService.Delete(id);

            // Assert
            productRepositoryMoq.Verify(x => x.Delete(id), Times.Once);
        }
    }
}
