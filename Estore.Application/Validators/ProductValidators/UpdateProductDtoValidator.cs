using Estore.Application.DTOS.Product;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estore.Application.Validators.ProductValidators
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

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

            //When(x => x.Images != null, () =>
            //{
            //    RuleForEach(x => x.Images).Must(file =>
            //    {
            //        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            //        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            //        return allowedExtensions.Contains(extension);
            //    }).WithMessage("Only JPG, JPEG, and PNG files are allowed.");

            //    RuleForEach(x => x.Images).Must(file => file.Length <= 15 * 1024 * 1024)
            //        .WithMessage("Each image must be less than 15MB.");
            //});
        }

    }
}
