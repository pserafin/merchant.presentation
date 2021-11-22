using System.Collections.Generic;
using System.Threading.Tasks;
using Merchant.CORE.Dtos;

namespace Merchant.CORE.IServices
{
    public interface IProductService
    {
        Task<ProductDto> Get(int id, bool enabledOnly = false);
        Task<IEnumerable<ProductDto>> List(bool enabledOnly = false);
        Task<IEnumerable<ProductDto>> List(IEnumerable<int> ids);
        Task<ProductDto> Add(ProductDto productDto);
        Task Update(ProductDto productDto);
        Task UpdateMany(IEnumerable<ProductDto> productDto);
        Task Delete(int id);
    }
}
