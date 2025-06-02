using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators;

public class ProductAddRequestValidator : AbstractValidator<ProductAddRequest>
{
    public ProductAddRequestValidator()
    {
        RuleFor(req => req.ProductName)
            .NotEmpty().WithMessage("Product Name can't be blank");

        RuleFor(req => req.Category)
            .IsInEnum().WithMessage("Invalid Category");

        RuleFor(req => req.UnitPrice)
            .InclusiveBetween(0, double.MaxValue).WithMessage($"UnitPrice must be between 0 and {double.MaxValue}");

        RuleFor(req => req.QuantityInStock)
            .InclusiveBetween(1, int.MaxValue).WithMessage($"QuantityInStock must be between 1 and {int.MaxValue}");
    }
}
