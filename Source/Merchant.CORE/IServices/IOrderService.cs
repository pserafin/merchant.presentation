using System.Collections.Generic;
using System.Threading.Tasks;
using Merchant.CORE.Dtos;
using Merchant.CORE.Models;

namespace Merchant.CORE.IServices
{
    public interface IOrderService
    {
        Task<OrderDto> Get(int id, int? userId = null);
        Task<OrderDto> GetCurrent(int userId);
        Task<IEnumerable<OrderDto>> List(int? userId = null);
        Task<OrderDto> Add(OrderDto orderDto);
        Task<OrderDto> Update(OrderDto orderDto);
        Task Delete(int id);
        Task<ValidationResult> Validate(OrderDto orderDto);
    }
}
