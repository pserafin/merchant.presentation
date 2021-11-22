using Merchant.CORE.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Merchant.CORE.EFObjects;
using Microsoft.AspNetCore.Identity;

namespace Merchant.CORE.IServices
{
    public interface IAccountService
    {
        Task<EFModels.User> Get(string name);

        Task<IEnumerable<string>> GetRoles(EFModels.User user);

        Task<IEnumerable<Role>> GetMappedRoles(EFModels.User user);

        Task<UserIdentity> GetUserIdentity(string name);

        Task<IdentityResult> CreateCustomer(UserLogin user);
    }
}
