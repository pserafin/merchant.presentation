using Merchant.CORE.IRepositories;
using Merchant.Order.EFObjects;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Merchant.CORE.EFModels;
using Merchant.CORE.Enums;


namespace Merchant.Order.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _dBContext;

        public OrderRepository(OrderContext dBContext)
        {
            _dBContext = dBContext;
        }

        public Task<CORE.EFModels.Order> Get(int id, int? userId = null) =>
            userId != null
            ? _dBContext.Orders
                .Include(x => x.Items)
                .Include(x => x.User)
                .Where(x => x.Id == id && x.UserId == userId)
                .AsNoTracking()
                .FirstOrDefaultAsync()
            : _dBContext.Orders
                .Include(x => x.Items)
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public Task<CORE.EFModels.Order> GetCurrent(int userId) =>
            _dBContext.Orders
                .Include(x => x.Items)
                .Where(x => x.UserId == userId && x.Status < OrderStatus.PaymentRegistered)
                .OrderByDescending(x => x.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();


        public async Task<IEnumerable<CORE.EFModels.Order>> List(int? userId = null) =>
            userId != null
            ? await _dBContext.Orders
                .Include(x => x.Items)
                .Where(x => x.UserId == userId && x.Status > OrderStatus.Validated)
                .OrderByDescending(x => x.Id)
                .ToListAsync()
            : await _dBContext.Orders
                .Include(x => x.Items)
                .Include(x => x.User)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

        public async Task<CORE.EFModels.Order> Add(CORE.EFModels.Order order)
        {
            _dBContext.Add(order);
            await _dBContext.SaveChangesAsync();

            return await _dBContext.Orders
                .Include(x => x.Items)
                .Include(x => x.User)
                .AsNoTracking()
                .FirstAsync(x => x.Id == order.Id);
        }

        public async Task<CORE.EFModels.Order> Update(CORE.EFModels.Order order)
        {
            var dbOrder = await _dBContext.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == order.Id);

            if (dbOrder == null)
            {
                return null;
            }

            UpdateOrder(dbOrder, order);
            AppendOrderItems(dbOrder, order);

            await _dBContext.SaveChangesAsync();

            return dbOrder;
        }

        public async Task Delete(int id)
        {
            var dbOrder = await _dBContext.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dbOrder == null)
            {
                return;
            }

            dbOrder.Items.Clear();
            _dBContext.Entry(dbOrder).State = EntityState.Deleted;

            await _dBContext.SaveChangesAsync();
        }

        public async Task SetStatus(int id, OrderStatus status)
        {
            var dbOrder = await _dBContext.Orders
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dbOrder == null)
            {
                return;
            }

            dbOrder.Status = status;
            _dBContext.Entry(dbOrder).State = EntityState.Modified;

            await _dBContext.SaveChangesAsync();
        }

        private void UpdateOrder(CORE.EFModels.Order destination, CORE.EFModels.Order source)
        {
            destination.Date = source.Date;
            destination.Status = source.Status;
            destination.TotalPrice = source.TotalPrice;
            _dBContext.Entry(destination).State = EntityState.Modified;
        }

        private void AppendOrderItems(CORE.EFModels.Order destination, CORE.EFModels.Order source)
        {
            destination.Items.Clear();
            foreach (var item in source.Items)
            {
                destination.Items.Add(new OrderItem
                {
                    Name = item.Name,
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }
        }


    }
}
