using Estore.Application.Common.GeneralResult;
using Estore.Application.DTOS.Product;
using Estore.Application.Exceptions;
using Estore.Application.Interfaces;
using Estore.Application.Utiles.HandleFiles;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Estore.API.Controllers
{
    [ApiController]
    [Route("api/products/{productId:guid}/images")]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _productImageService;
        private readonly IFileService _fileService;

        public ProductImageController(IProductImageService productImageService, IFileService fileService)
        {
            _productImageService = productImageService;
            _fileService = fileService;
        }


        [HttpGet]
        public async Task<Results<Ok<ApiResponse<IEnumerable<ProductImageDto>>>, NotFound<ApiResponse<object>>>> GetProductImages(Guid productId)
        {
            try
            {
                var images = await _productImageService.GetImgesByProductIdAsync(productId);
                return TypedResults.Ok(ApiResponse<IEnumerable<ProductImageDto>>.SuccessResponse(images, "Product images retrieved successfully"));
            }
            catch (NotFoundException ex)
            {
                return TypedResults.NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<Results<Created<ApiResponse<ProductImageDto>>, BadRequest<ApiResponse<object>>, NotFound<ApiResponse<object>>>> UploadProductImage(
            Guid productId,
            [FromForm] IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse("No image file provided"));
            }

            try
            {
                // Upload file first
                var uploadResult = await _fileService.UploadFileAsync(imageFile);

                // Create the DTO for the service
                var productImageCreateDto = new ProductImageCreateDto
                {
                    ProductId = productId,
                    FileUrl = uploadResult.FileUrl,
                    FileName = imageFile.FileName
                };

                // Save image metadata
                var productImage = await _productImageService.UploadImageAsync(productId, productImageCreateDto);

                var location = $"/api/products/{productId}/images/{productImage.Id}"; // details in header 
                return TypedResults.Created(location, ApiResponse<ProductImageDto>.SuccessResponse(productImage, "Image uploaded successfully"));
            }
            catch (ArgumentException ex)
            {
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (NotFoundException ex)
            {
                return TypedResults.NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }


        [HttpGet("{imageId:guid}")]
        public async Task<Results<Ok<ApiResponse<ProductImageDto>>, NotFound<ApiResponse<object>>>> GetProductImageById(Guid productId, Guid imageId)
        {
            try
            {
                var image = await _productImageService.GetImageByIdAsync(imageId);

                // Verify the image belongs to the specified product
                if (image.ProductId != productId)
                {
                    return TypedResults.NotFound(ApiResponse<object>.ErrorResponse("Image not found for this product"));
                }

                return TypedResults.Ok(ApiResponse<ProductImageDto>.SuccessResponse(image, "Product image retrieved successfully"));
            }
            catch (NotFoundException ex)
            {
                return TypedResults.NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }

        }
        [HttpDelete("{imageId:guid}")]
        public async Task<Results<Ok<ApiResponse<object>>, NotFound<ApiResponse<object>>>> DeleteProductImage(Guid productId, Guid imageId)
        {
            try
            {
                // First verify the image exists and belongs to the product
                var image = await _productImageService.GetImageByIdAsync(imageId);
                if (image.ProductId != productId)
                {
                    return TypedResults.NotFound(ApiResponse<object>.ErrorResponse("Image not found for this product"));
                }

                await _productImageService.DeleteImageAsync(imageId);
                return TypedResults.Ok(ApiResponse<object>.SuccessResponse("Product image deleted successfully"));
            }
            catch (NotFoundException ex)
            {
                return TypedResults.NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

    }
}
