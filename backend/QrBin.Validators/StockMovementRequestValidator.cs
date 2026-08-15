using FluentValidation;
using QrBin.ViewModels.Inventory;

namespace QrBin.Validators;

public class StockMovementRequestValidator : AbstractValidator<StockMovementRequest>
{
    public StockMovementRequestValidator()
    {
        RuleFor(x => x.QuantityDelta).NotEqual(0).WithMessage("Enter a non-zero quantity.");
    }
}
