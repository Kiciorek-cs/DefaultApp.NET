using Demo.BLL.Features.User.Commands.ConfirmAccountByUserId;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.BLL.Validators.Token;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class ConfirmAccountByUserIdCommandValidator : CustomValidator<ConfirmAccountByUserIdCommand>
{
    public ConfirmAccountByUserIdCommandValidator(IPermissionUnitOfWork permissionUnitOfWork, IToken token,
        IClock clock)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);
        var tokenCommonValidator = new TokenCommonValidator(token, permissionUnitOfWork, clock);

        RuleFor(x => x.UserId)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "UserId", context));

        RuleFor(x => x.UserId)
            .Custom((x, context) => userCommonValidator.UserExistById(x, context));

        RuleFor(x => x.UserId)
            .Custom((x, context) => userCommonValidator.UserCanBeConfirmed(x, context));
    }
}