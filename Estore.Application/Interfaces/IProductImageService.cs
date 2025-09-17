using Estore.Application.DTOS.Product;

namespace Estore.Application.Interfaces
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageDto>> GetImgesByProductIdAsync(Guid productId);
        Task<ProductImageDto> UploadImageAsync(Guid productId, ProductImageCreateDto dto);
        Task<bool> DeleteImageAsync(Guid imageId);
        Task<ProductImageDto?> GetImageByIdAsync(Guid imageId);
    }
}
