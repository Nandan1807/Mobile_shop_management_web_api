using FluentValidation;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.FluentValidation
{
    public class CategoryModelValidator : AbstractValidator<CategoryModel>
    {
        public CategoryModelValidator()
        {
            RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Category Name is required.")
                .Length(2, 50).WithMessage("Category Name must be between 2 and 50 characters.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.")
                .Length(2, 200).WithMessage("Description must be between 2 and 200 characters.");
        }
    }
}