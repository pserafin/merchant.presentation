using AutoMapper;
using Merchant.API.ViewModels;
using Merchant.CORE.EFModels;
using Merchant.CORE.EFObjects;
using Merchant.CORE.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Merchant.CORE.Extensions;

namespace Merchant.API.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private const string SetCookieName = "Set-Cookie";

        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly SignInManager<User> _signInManager;

        public AccountController(IMapper mapper, IAccountService accountService, SignInManager<User> signinManager)
        {
            _mapper = mapper;
            _accountService = accountService;
            _signInManager = signinManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            if (ModelState.IsValid)
            {
                var appUser = await _accountService.Get(login.Name);
                if (appUser != null)
                {
                    await _signInManager.SignOutAsync();

                    var result = await _signInManager.PasswordSignInAsync(appUser, login.Password, false, false);
                    if (result.Succeeded)
                    {
                        var user = await PrepareUserViewModel(appUser);

                        return Ok(user);
                    }
                }

                return Unauthorized();
            }

            return Unauthorized();
        }

        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            Response.Cookies.Delete(SetCookieName);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("get")]
        public async Task<IActionResult> Get()
        {
            var userName = User.GetName();
            var userIdentity = await _accountService.GetUserIdentity(userName);
            
            return Ok(_mapper.Map<UserIdentityViewModel>(userIdentity));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("heartbeat")]
        public IActionResult Heartbeat()
        {

            return Ok("bum bum, bum bum, bum bum...");
        }

        private async Task<UserIdentityViewModel> PrepareUserViewModel(User appUser)
        {
            var viewModel = _mapper.Map<UserIdentityViewModel>(appUser);
            viewModel.Roles = await _accountService.GetRoles(appUser);

            return viewModel;
        }
    }
}
