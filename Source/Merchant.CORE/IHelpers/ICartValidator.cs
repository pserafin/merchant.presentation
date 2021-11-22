using System.Threading.Tasks;
using Merchant.CORE.Dtos;
using Merchant.CORE.Models;

namespace Merchant.CORE.IHelpers
{
    public interface ICartValidator
    {
        Task<ValidationResult> ValidateOrder(OrderDto order);
        decimal CalculateTotal(OrderDto order);
    }
}
