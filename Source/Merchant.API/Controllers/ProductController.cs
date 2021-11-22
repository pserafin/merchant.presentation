using System.Collections.Generic;
using Merchant.CORE.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Merchant.API.ViewModels;
using Merchant.CORE.Dtos;
using Merchant.CORE.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Merchant.API.Controllers
{
    public class ProductController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        
        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }
        
        [HttpGet]
        [Route("product/{id}")]
        [Authorize(Roles = "Administrator,Customer")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.Get(id, id != 0 && User.IsCustomerOnly());

            return Ok(_mapper.Map<ProductViewModel>(product));
        }

        [HttpGet]
        [Route("product/list")]
        [Authorize(Roles = "Administrator,Customer")]
        public async Task<IActionResult> List()
        {
            var products = await _productService.List(User.IsCustomerOnly());

            return Ok(_mapper.Map<IEnumerable<ProductViewModel>>(products));
        }

        [HttpPost]
        [Route("product/add")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Post([FromBody] ProductViewModel product)
        {
            if (product == null)
            {
                return NoContent();
            }

            var newProduct = await _productService.Add(_mapper.Map<ProductDto>(product));

            return Ok(_mapper.Map<ProductViewModel>(newProduct));
        }

        [HttpPut]
        [Route("product/update")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Put([FromBody] ProductViewModel product)
        {
            if (product == null)
            {
                return NoContent();
            }

            await _productService.Update(_mapper.Map<ProductDto>(product));

            return Ok();
        }

        [HttpDelete]
        [Route("product/delete/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Put(int id)
        {
            await _productService.Delete(id);

            return Ok();
        }
    }
}
