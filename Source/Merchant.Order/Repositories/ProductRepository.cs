using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Merchant.CORE.EFModels;
using Merchant.CORE.IRepositories;
using Merchant.Order.EFObjects;

namespace Merchant.Order.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly OrderContext _dBContext;

        public ProductRepository(OrderContext dBContext)
        {
            _dBContext = dBContext;
        }

        public Task<Product> Get(int id, bool enabledOnly = false) =>
            enabledOnly
                ? _dBContext.Products
                    .Where(x => x.Id == id && x.IsEnabled)
                    .AsNoTracking()
                    .FirstOrDefaultAsync()
                : _dBContext.Products
                    .Where(x => x.Id == id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();


        public async Task<IEnumerable<Product>> List(bool enabledOnly = false) =>
            enabledOnly
                ? await _dBContext.Products
                    .Where(x => x.IsEnabled)
                    .ToListAsync()
                : await _dBContext.Products
                    .ToListAsync();

        public async Task<IEnumerable<Product>> List(IEnumerable<int> ids) =>
            await _dBContext.Products
                .Where(x => ids.Contains(x.Id))
                .AsNoTracking()
                .ToListAsync();

        public async Task<Product> Add(Product product)
        {
            _dBContext.Add(product);
            await _dBContext.SaveChangesAsync();

            return product;
        }

        public async Task Update(Product product)
        {
            _dBContext.Entry(product).State = EntityState.Modified;
            await _dBContext.SaveChangesAsync();
        }

        public async Task UpdateMany(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                _dBContext.Entry(product).State = EntityState.Modified;
            }
            
            await _dBContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var product = await Get(id);

            if (product != null)
            {
                _dBContext.Entry(product).State = EntityState.Deleted;
                await _dBContext.SaveChangesAsync();
            }
        }
    }
}
