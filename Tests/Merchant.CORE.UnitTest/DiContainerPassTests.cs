using AutoMapper;
using Merchant.API.Setup;
using Xunit;

namespace Merchant.CORE.UnitTest
{
    public class DiContainerPassTests : BaseFixture
    {
        [Fact]
        public void ConfigureDIContainer_WhenCongigured_ConfigurationIsValid()
        {
            //Arrange

            //Act
            var config = new MapperConfiguration(opts => opts.AddProfile(new AutoMapperProfile()));

            //Assert
            config.AssertConfigurationIsValid();
        }
    }
}
