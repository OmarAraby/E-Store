using AutoMapper;
using Estore.Application.DTOS.Product;
using Estore.Application.Exceptions;
using Estore.Application.Interfaces;
using Estore.Domain.Entities;
using Estore.Domain.Repositories;

namespace Estore.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductImageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> DeleteImageAsync(Guid imageId)
        {
            var image = await _unitOfWork.ProductImageRepository.GetByIdAsync(imageId);
            if (image == null)
                throw new NotFoundException($"Image with ID '{imageId}' not found");

            await _unitOfWork.ProductImageRepository.DeleteAsync(image);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ProductImageDto?> GetImageByIdAsync(Guid imageId)
        {
            var image = await _unitOfWork.ProductImageRepository.GetByIdAsync(imageId);
            if (image == null)
                throw new NotFoundException($"Image with ID '{imageId}' not found");

            return _mapper.Map<ProductImageDto>(image);
        }

        public async Task<IEnumerable<ProductImageDto>> GetImgesByProductIdAsync(Guid productId)
        {
            var productExists = await _unitOfWork.ProductRepository.ExistsAsync(productId);
            if (!productExists)
                throw new NotFoundException($"Product with ID '{productId}' not found");

            var images = await _unitOfWork.ProductImageRepository.GetByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ProductImageDto>>(images);
        }

        public async Task<ProductImageDto> UploadImageAsync(Guid productId, ProductImageCreateDto dto)
        {
            var productExists = await _unitOfWork.ProductRepository.ExistsAsync(productId);
            if (!productExists)
                throw new NotFoundException($"Product with ID '{productId}' not found");

            if (dto.ProductId != productId)
                throw new BadRequestException("Product ID in request body must match the route parameter");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.FileUrl))
                throw new BadRequestException("File URL is required");

            if (string.IsNullOrWhiteSpace(dto.FileName))
                throw new BadRequestException("File name is required");

            var productImage = new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                FileName = dto.FileName,
                ImagePath = dto.FileUrl,
                UploadedAt = DateTime.UtcNow
            };

            await _unitOfWork.ProductImageRepository.AddAsync(productImage);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductImageDto>(productImage);


        }
    }
}
