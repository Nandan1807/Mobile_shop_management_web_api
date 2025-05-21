using FluentValidation;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.FluentValidation
{
    public class InvoiceItemModelValidator : AbstractValidator<InvoiceItemModel>
    {
        public InvoiceItemModelValidator()
        {
            RuleFor(x => x.InvoiceId).NotEmpty().WithMessage("Invoice ID is required.");
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}