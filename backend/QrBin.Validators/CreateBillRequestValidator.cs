using FluentValidation;
using QrBin.ViewModels.Billing;

namespace QrBin.Validators;

public class CreateBillRequestValidator : AbstractValidator<CreateBillRequest>
{
    public CreateBillRequestValidator()
    {
        RuleFor(x => x.CustomerName).MaximumLength(200);
        RuleFor(x => x.CustomerPhone).MaximumLength(30);
        RuleFor(x => x.Items).NotEmpty().WithMessage("Add at least one product before creating the bill.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
