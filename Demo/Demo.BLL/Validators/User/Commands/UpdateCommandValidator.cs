using Demo.BLL.Features.User.Commands.Update;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class UpdateCommandValidator : CustomValidator<UpdateCommand>
{
    public UpdateCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => userCommonValidator.UserExistById(x, context));

        RuleFor(x => x.FirstName)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "FirstName", context));

        RuleFor(x => x.LastName)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "LastName", context));

        RuleFor(x => x.DateOfBirth)
            .Custom((x, context) => CommonValidator.DateOfBirthIsValid(x, context));

        RuleFor(x => x.LoginName)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "LoginName", context));

        RuleFor(x => new { x.LoginName, x.Id })
            .Custom((x, context) => userCommonValidator.UserExistByUserName(x.LoginName, context, x.Id));

        RuleFor(x => x.Gender)
            .Custom((x, context) => CommonValidator.WhetherTheEnumIsCorrect(x, "Gender", context));
    }
}