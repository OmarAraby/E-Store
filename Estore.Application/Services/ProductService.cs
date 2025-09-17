using AutoMapper;
using Estore.Application.DTOS.Product;
using Estore.Application.Exceptions;
using Estore.Application.Interfaces;
using Estore.Domain.Entities;
using Estore.Domain.Repositories;

namespace Estore.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
        {
            var existingProduct = await _unitOfWork.ProductRepository.GetByProductCodeAsync(createProductDto.ProductCode);
            if (existingProduct != null)
                throw new ConflictException($"Product with code '{createProductDto.ProductCode}' already exists");

            var product = _mapper.Map<Product>(createProductDto);
            product.ImagePath = string.Empty;

            var createdProduct = await _unitOfWork.ProductRepository.CreateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with ID '{id}' not found");

            await _unitOfWork.ProductRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return true; 
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category)
        {
            var products = await _unitOfWork.ProductRepository.GetByCategoryAsync(category);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with ID '{id}' not found");

            return _mapper.Map<ProductDto>(product); ;
        }

        public async Task<ProductDto> GetByProductCodeAsync(string productCode)
        {
            var product = await _unitOfWork.ProductRepository.GetByProductCodeAsync(productCode);
            if (product == null)
                throw new NotFoundException($"Product with code '{productCode}' not found");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateProductDto)
        {
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (existingProduct == null)
                throw new NotFoundException($"Product with ID '{id}' not found");

            _mapper.Map(updateProductDto, existingProduct);

            var updatedProduct = await _unitOfWork.ProductRepository.UpdateAsync(existingProduct);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductDto>(updatedProduct);
        }
    }
}
