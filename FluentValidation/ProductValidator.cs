using FluentValidation;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.FluentValidation
{
    public class ProductModelValidator : AbstractValidator<ProductModel>
    {
        public ProductModelValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product Name is required.")
                .Length(2, 100).WithMessage("Product Name must be between 2 and 100 characters.");
            RuleFor(x => x.ProductPrice).GreaterThan(0).WithMessage("Product Price must be greater than 0.");
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("Stock Quantity must be a non-negative number.");
            RuleFor(x => x.Status).NotEmpty().WithMessage("Product Status is required.");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Category ID must be greater than 0.");
            RuleFor(x => x.ProductBrandId).GreaterThan(0).WithMessage("Brand ID must be greater than 0.");

        }
    }
}