using FluentValidation;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.FluentValidation
{
    public class InvoiceModelValidator : AbstractValidator<InvoiceModel>
    {
        public InvoiceModelValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Payment Method is required.");
            RuleFor(x => x.PaymentStatus).NotEmpty().WithMessage("Payment Status is required.");
        }
    }
}