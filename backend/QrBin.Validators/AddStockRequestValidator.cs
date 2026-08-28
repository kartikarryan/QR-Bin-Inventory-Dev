using FluentValidation;
using QrBin.ViewModels.Products;

namespace QrBin.Validators;

public class AddStockRequestValidator : AbstractValidator<AddStockRequest>
{
    public AddStockRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
