using FluentValidation;
using QrBin.ViewModels.Inventory;

namespace QrBin.Validators;

public class CreatePartRequestValidator : AbstractValidator<CreatePartRequest>
{
    public CreatePartRequestValidator()
    {
        RuleFor(x => x.PartName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PartNumber).MaximumLength(100);
        RuleFor(x => x.BinCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CurrentStock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinimumStock).GreaterThanOrEqualTo(0);
    }
}
