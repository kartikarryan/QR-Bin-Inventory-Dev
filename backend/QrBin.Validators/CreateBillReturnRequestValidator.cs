using FluentValidation;
using QrBin.ViewModels.Billing;

namespace QrBin.Validators;

public class CreateBillReturnRequestValidator : AbstractValidator<CreateBillReturnRequest>
{
    public CreateBillReturnRequestValidator()
    {
        RuleFor(x => x.BillItemId).GreaterThan(0);
        RuleFor(x => x.ReturnedQuantity).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500);

        // Replacement product and quantity are an all-or-nothing pair — an exchange needs both.
        RuleFor(x => x.ReplacementQuantity)
            .NotNull()
            .GreaterThan(0)
            .When(x => x.ReplacementProductId.HasValue)
            .WithMessage("Enter a quantity for the replacement product.");

        RuleFor(x => x.ReplacementProductId)
            .NotNull()
            .GreaterThan(0)
            .When(x => x.ReplacementQuantity.HasValue)
            .WithMessage("Choose a replacement product.");
    }
}
