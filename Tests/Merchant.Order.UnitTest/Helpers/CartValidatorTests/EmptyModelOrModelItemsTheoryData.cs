using Merchant.CORE.Dtos;
using Xunit;

namespace Merchant.Order.UnitTest.Helpers.CartValidatorTests
{
    public class EmptyModelOrModelItemsTheoryData : TheoryData<OrderDto>
    {
        public EmptyModelOrModelItemsTheoryData()
        {
            Add(null);
            Add(new OrderDto { Items = null, Id = 0 });
            Add(new OrderDto {Id = 0});
        }
    }
}
