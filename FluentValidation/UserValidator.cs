using FluentValidation;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.FluentValidation
{
    public class UserModelValidator : AbstractValidator<UserModel>
    {
        public UserModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("User Name is required.")
                .Length(2, 50).WithMessage("User Name must be between 2 and 50 characters.");
            RuleFor(x => x.UserEmail).NotEmpty().WithMessage("User Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                .Length(6, 20).WithMessage("Password must be between 6 and 20 characters.");
            RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required.");
            RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required.");
        }
    }
    
    public class UserAuthValidator : AbstractValidator<UserAuthModel>
    {
        public UserAuthValidator()
        {
            RuleFor(x => x.UserEmail).NotEmpty().WithMessage("User Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                .Length(6, 20).WithMessage("Password must be between 6 and 20 characters.");
        }
    }
}