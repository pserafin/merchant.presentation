using AutoMapper;
using Merchant.CORE.Dtos;
using Merchant.CORE.EFModels;
using Merchant.CORE.IRepositories;
using Merchant.CORE.IServices;
using Merchant.CORE.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Merchant.Order.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        
        public ProductService(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<ProductDto> Get(int id, bool enabledOnly = false)
        {
            try
            {
                var product = await _productRepository.Get(id, enabledOnly);

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.RetrievalError, "product data"), e);
            }
        }

        public async Task<IEnumerable<ProductDto>> List(bool enabledOnly = false)
        {
            try
            {
                var products = await _productRepository.List(enabledOnly);

                return _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.RetrievalError, "product list"), e);
            }
        }

        public async Task<IEnumerable<ProductDto>> List(IEnumerable<int> ids)
        {
            try
            {
                var products = await _productRepository.List(ids);

                return _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.RetrievalError, "product list"), e);
            }
        }

        public async Task<ProductDto> Add(ProductDto productDto)
        {
            try
            {
                NormalizeModel(productDto);

                var product = await _productRepository.Add(_mapper.Map<Product>(productDto));

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.ModificationError, "add", "new product"), e);
            }
        }

        public async Task Update(ProductDto productDto)
        {
            try
            {
                await _productRepository.Update(_mapper.Map<Product>(productDto));
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.ModificationError, "update", "product"), e);
            }
        }

        public async Task UpdateMany(IEnumerable<ProductDto> productsDto)
        {
            try
            {
                await _productRepository.UpdateMany(_mapper.Map<IEnumerable<Product>>(productsDto));
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.ModificationError, "update", "product"), e);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _productRepository.Delete(id);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("DELETE"))
                {
                    throw new UIException(ErrorMessages.ProductDeleteError, e);
                }
                throw new UIException(string.Format(ErrorMessages.ModificationError, "delete", "product"), e);
            }
            catch (Exception e)
            {
                throw new UIException(string.Format(ErrorMessages.ModificationError, "delete", "product"), e);
            }
        }

        private void NormalizeModel(ProductDto product)
        {
            product.Id = 0;
        }
    }
}
