namespace E_Shop.Application.Validators
{
    public class AddCategoryValidator : AbstractValidator<CategoryAddRequest>
    {
        public AddCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

            RuleFor(x => x.Image)
            .Must(file => file == null || file.ContentType.StartsWith("image/"))
            .WithMessage("Only image files are allowed.");

        }
    }

}
