using E_Shop.Application.Dtos.ProductDtos;

namespace E_Shop.Application.Validators
{
    public class ProductAddValidator : AbstractValidator<ProductAddRequest>
    {
        public ProductAddValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

            RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.Images)
                .Must(images => images != null && images.All(image => image.Length > 0))
                .WithMessage("At least one valid image is required.");
        }
    }
}
