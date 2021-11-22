using System.Threading.Tasks;
using AutoMapper;
using Merchant.API.ViewModels;
using Merchant.CORE.EFObjects;
using Merchant.CORE.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Merchant.API.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;

        public UserController(IMapper mapper, IAccountService accountService)
        {
            _mapper = mapper;
            _accountService = accountService;
        }

        [HttpPost]
        [Route("user/create")]
        public async Task<IActionResult> Create(UserLogin user)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(user.Email))
            {
                IdentityResult result = await _accountService.CreateCustomer(user);
                if (result.Succeeded)
                {
                    var userIdentity = await _accountService.GetUserIdentity(user.Name);
                    return Ok(_mapper.Map<UserIdentityViewModel>(userIdentity));
                }

                return StatusCode(500, result.Errors);
            }

            return StatusCode(500, "Invalid model");
        }
    }
}
