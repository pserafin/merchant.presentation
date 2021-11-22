using System.Collections.Generic;
using System.Threading.Tasks;
using Merchant.CORE.EFModels;
using Merchant.CORE.Enums;

namespace Merchant.CORE.IRepositories
{
    public interface IOrderRepository
    {
        Task<Order> Get(int id, int? userId = null);
        Task<Order> GetCurrent(int userId);
        Task<IEnumerable<Order>> List(int? userId = null);
        Task<Order> Add(Order order);
        Task<Order> Update(Order order);
        Task SetStatus(int id, OrderStatus status);
        Task Delete(int id);
    }
}
