using FluentValidation;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.FluentValidation
{
    public class DropdownItemModelValidator : AbstractValidator<DropdownItemModel>
    {
        public DropdownItemModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");
        }
    }
}