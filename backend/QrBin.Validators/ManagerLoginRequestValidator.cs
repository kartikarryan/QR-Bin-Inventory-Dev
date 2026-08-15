using FluentValidation;
using QrBin.ViewModels.Auth;

namespace QrBin.Validators;

public class ManagerLoginRequestValidator : AbstractValidator<ManagerLoginRequest>
{
    public ManagerLoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
