using FluentValidation;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.FluentValidation
{
    public class BrandModelValidator : AbstractValidator<BrandModel>
    {
        public BrandModelValidator()
        {
            RuleFor(x => x.BrandName).NotEmpty().WithMessage("Brand Name is required.")
                .Length(2, 50).WithMessage("Brand Name must be between 2 and 50 characters.");
        }
    }
}