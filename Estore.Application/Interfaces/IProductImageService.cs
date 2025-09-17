using Estore.Application.DTOS.Product;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estore.Application.Interfaces
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageDto>> GetByProductIdAsync(Guid productId);
        Task<ProductImageDto> AddImageAsync(Guid productId, IFormFile imageFile);
        Task<bool> DeleteImageAsync(Guid imageId);
        Task<ProductImageDto?> GetImageByIdAsync(Guid imageId);
    }
}
