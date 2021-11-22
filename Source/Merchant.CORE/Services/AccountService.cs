using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Merchant.CORE.EFObjects;
using Merchant.CORE.Enums;
using Merchant.CORE.IServices;
using Microsoft.AspNetCore.Identity;

namespace Merchant.CORE.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<EFModels.User> _userManager;

        public AccountService(IMapper mapper, UserManager<EFModels.User> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public Task<EFModels.User> Get(string name) => 
            _userManager.FindByNameAsync(name);

        public async Task<IEnumerable<string>> GetRoles(EFModels.User user) => 
            await _userManager.GetRolesAsync(user);

        public async Task<IEnumerable<Role>> GetMappedRoles(EFModels.User user)
        {
            var roles = await GetRoles(user);
            if (!roles.Any())
            {
                return Enumerable.Empty<Role>();
            }

            return roles.Select(x => (Role) Enum.Parse(typeof(Role), x)).ToArray();
        }

        public async Task<UserIdentity> GetUserIdentity(string name)
        {
            var user = await Get(name);
            if (user == null)
            {
                return null;
            }

            var model = _mapper.Map<UserIdentity>(user);
            model.Roles = await GetMappedRoles(user);

            return model;
        }

        public async Task<IdentityResult> CreateCustomer(UserLogin user)
        {
            var appUser = new EFModels.User
            {
                UserName = user.Name,
                Email = user.Email,
                FirstName = "Firstname",
                LastName = "Lastname"
            };

            IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
            if (result.Succeeded)
            {
                var addedUser = await Get(user.Name);
                await _userManager.AddToRoleAsync(addedUser, Role.Customer.ToString());
            }

            return result;
        }
    }
}
