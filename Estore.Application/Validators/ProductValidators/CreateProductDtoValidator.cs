using Estore.Application.DTOS.Product;
using FluentValidation;

namespace Estore.Application.Validators.ProductValidators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

            RuleFor(x => x.ProductCode)
                .NotEmpty().WithMessage("Product code is required.")
                .MaximumLength(20).WithMessage("Product code must not exceed 20 characters.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.MinimumQuantity)
                .GreaterThan(0).WithMessage("Minimum quantity must be at least 1.");

            RuleFor(x => x.DiscountRate)
                .GreaterThanOrEqualTo(0).WithMessage("Discount rate cannot be negative.")
                .LessThanOrEqualTo(100).WithMessage("Discount rate cannot exceed 100%.");

            
        }
    }

}
