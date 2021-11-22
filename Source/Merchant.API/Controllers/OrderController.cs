using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Merchant.API.ViewModels;
using Merchant.CORE.Dtos;
using Merchant.CORE.Extensions;
using Merchant.CORE.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Merchant.API.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public OrderController(IMapper mapper, IOrderService orderService)
        {
            _mapper = mapper;
            _orderService = orderService;
        }

        [HttpGet]
        [Route("order/{id}")]
        [Authorize(Roles = "Administrator,Customer")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.IsCustomerOnly() ? User.GetId() : null;

            var order = await _orderService.Get(id, userId);

            return Ok(_mapper.Map<OrderViewModel>(order));
        }

        [HttpGet]
        [Route("order/current")]
        [Authorize(Roles = "Administrator,Customer")]
        public async Task<IActionResult> GetCurrent()
        {
            var order = await _orderService.GetCurrent(User.GetId().Value);

            return Ok(_mapper.Map<OrderViewModel>(order));
        }

        [HttpGet]
        [Route("order/list")]
        [Authorize(Roles = "Administrator,Customer")]
        public async Task<IActionResult> List()
        {
            var userId = User.IsCustomerOnly() ? User.GetId() : null;

            var orders = await _orderService.List(userId);

            return Ok(_mapper.Map<IEnumerable<OrderViewModel>>(orders));
        }

        [HttpPost]
        [Route("order/validate")]
        [Authorize(Roles = "Administrator,Customer")]
        public async Task<IActionResult> Validate([FromBody] OrderViewModel order)
        {
            if (order == null)
            {
                return NoContent();
            }

            var result = await _orderService.Validate(_mapper.Map<OrderDto>(order));

            return Ok(_mapper.Map<ValidationResultViewModel>(result));
        }

        [HttpPost]
        [Route("order/add")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Post([FromBody] OrderViewModel order)
        {
            if (order == null)
            {
                return NoContent();
            }

            var newOrder = await _orderService.Add(_mapper.Map<OrderDto>(order));

            return Ok(_mapper.Map<OrderViewModel>(newOrder));
        }

        [HttpPut]
        [Route("order/update")]
        [Authorize(Roles = "Administrator,Customer")]
        public async Task<IActionResult> Put([FromBody] OrderViewModel order)
        {
            if (order == null)
            {
                return NoContent();
            }

            if (User.IsCustomerOnly())
            {
                var dbOrder = await _orderService.Get(order.Id, User.GetId());
                if (dbOrder == null)
                {
                    return Unauthorized(new
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        message = "Unauthorized Order access"
                    });
                }
            }

            var updatedOrder = await _orderService.Update(_mapper.Map<OrderDto>(order));

            return Ok(_mapper.Map<OrderViewModel>(updatedOrder));
        }

        [HttpDelete]
        [Route("order/delete/{id}")]
        [Authorize(Roles = "Administrator,Customer")]
        public async Task<IActionResult> Put(int id)
        {
            if (User.IsCustomerOnly())
            {
                var dbOrder = await _orderService.Get(id, User.GetId());
                if (dbOrder == null)
                {
                    return Unauthorized(new
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        message = "Unauthorized Order access"
                    });
                }
            }

            await _orderService.Delete(id);

            return Ok();
        }
    }
}
