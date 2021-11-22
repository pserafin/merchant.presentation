using System.Collections.Generic;
using System.Threading.Tasks;
using Merchant.CORE.EFModels;

namespace Merchant.CORE.IRepositories
{
    public interface IProductRepository
    {
        Task<Product> Get(int id, bool enabledOnly = false);
        Task<IEnumerable<Product>> List(bool enabledOnly = false);
        Task<IEnumerable<Product>> List(IEnumerable<int> ids);
        Task<Product> Add(Product product);
        Task Update(Product product);
        Task UpdateMany(IEnumerable<Product> products);
        Task Delete(int id);
    }
}
