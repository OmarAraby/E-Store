using Estore.Application.Common.GeneralResult;
using Estore.Application.DTOS.Product;
using Estore.Application.Exceptions;
using Estore.Application.Interfaces;
using Estore.Domain.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Estore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController: ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<CreateProductDto> _createProductValidator;
        private readonly IValidator<UpdateProductDto> _updateProductValidator;

        public ProductController(IProductService productService, IValidator<CreateProductDto> createProductValidator, IValidator<UpdateProductDto> updateProductValidator)
        {
            _productService = productService;
            _createProductValidator = createProductValidator;
            _updateProductValidator = updateProductValidator;
        }

        [HttpGet]
        public async Task<Ok<ApiResponse<IEnumerable<ProductDto>>>> GetAllProducts()
        {
            var products = await _productService.GetAllAsync();
            return TypedResults.Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Products retrieved successfully"));
        }

        [HttpGet("{id:guid}")]
        public async Task<Results<Ok<ApiResponse<ProductDto>>, NotFound<ApiResponse<object>>>> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return TypedResults.Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product retrieved successfully"));
            }
            catch (NotFoundException ex)
            {
                return TypedResults.NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("code/{productCode}")]
        public async Task<Results<Ok<ApiResponse<ProductDto>>, NotFound<ApiResponse<object>>>> GetProductByCode(string productCode)
        {
            try
            {
                var product = await _productService.GetByProductCodeAsync(productCode);
                return TypedResults.Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product retrieved successfully"));
            }
            catch (NotFoundException ex)
            {
                return TypedResults.NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("category/{category}")]
        public async Task<Ok<ApiResponse<IEnumerable<ProductDto>>>> GetProductsByCategory(string category)
        {
            var products = await _productService.GetByCategoryAsync(category);
            return TypedResults.Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, $"Products in '{category}' category retrieved successfully"));
        }

        [HttpPost]
        public async Task<Results<Created<ApiResponse<ProductDto>>, BadRequest<ApiResponse<object>>, Conflict<ApiResponse<object>>>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var validationResult = await _createProductValidator.ValidateAsync(createProductDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
            }

            try
            {
                var product = await _productService.CreateAsync(createProductDto);
                var location = $"/api/product/{product.Id}";  // for product details in header
                return TypedResults.Created(location, ApiResponse<ProductDto>.SuccessResponse(product, "Product created successfully"));
            }
            catch (ConflictException ex)
            {
                return TypedResults.Conflict(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<Results<Ok<ApiResponse<ProductDto>>, BadRequest<ApiResponse<object>>, NotFound<ApiResponse<object>>>> UpdateProduct(Guid id, [FromBody] UpdateProductDto updateProductDto)
        {
            var validationResult = await _updateProductValidator.ValidateAsync(updateProductDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
            }

            try
            {
                var product = await _productService.UpdateAsync(id, updateProductDto);
                return TypedResults.Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product updated successfully"));
            }
            catch (NotFoundException ex)
            {
                return TypedResults.NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<Results<Ok<ApiResponse<object>>, NotFound<ApiResponse<object>>>> DeleteProduct(Guid id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                return TypedResults.Ok(ApiResponse<object>.SuccessResponse("Product deleted successfully"));
            }
            catch (NotFoundException ex)
            {
                return TypedResults.NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }


        // get products with pagination mechnism 
        [HttpGet("paginated")]
        public async Task<Ok<ApiResponse<PageList<ProductDto>>>> GetPaginatedProducts([FromQuery]ProductQueryParams queryParams)
        {
            var paginatedProducts = await _productService.GetPaginatedAsync(queryParams);
            return TypedResults.Ok(ApiResponse<PageList<ProductDto>>.SuccessResponse(paginatedProducts, "Products Retrive Successfully"));
        }
    }
}
