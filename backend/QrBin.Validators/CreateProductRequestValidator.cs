using FluentValidation;
using QrBin.ViewModels.Products;

namespace QrBin.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).MaximumLength(100);
        RuleFor(x => x.HsnCode).MaximumLength(20);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(20);
        RuleFor(x => x.SellingPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.GstRate).InclusiveBetween(0, 100);
        RuleFor(x => x.OpeningStock).GreaterThanOrEqualTo(0);
    }
}
