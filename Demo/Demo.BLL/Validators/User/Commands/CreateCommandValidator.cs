using Demo.BLL.Features.User.Commands.Create;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Validators.Role;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class CreateCommandValidator : CustomValidator<CreateCommand>
{
    public CreateCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);
        var roleCommonValidator = new RoleCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.FirstName)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "FirstName", context));

        RuleFor(x => x.LastName)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "LastName", context));

        RuleFor(x => x.DateOfBirth)
            .Custom((x, context) => CommonValidator.DateOfBirthIsValid(x, context));

        RuleFor(x => x.EmailAddress)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "EmailAddress", context));

        RuleFor(x => x.EmailAddress)
            .Custom((x, context) => CommonValidator.EmailChecker(x, context));

        RuleFor(x => x.EmailAddress)
            .Custom((x, context) => userCommonValidator.UserNotExistByEmailAddress(x, context));

        RuleFor(x => x.LoginName)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "LoginName", context));

        RuleFor(x => x.LoginName)
            .Custom((x, context) => userCommonValidator.UserExistByUserName(x, context));

        RuleFor(x => x.Password)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Password", context));

        RuleFor(x => x.Password)
            .Custom((x, context) => CommonValidator.PasswordChecker(x, context));

        RuleFor(x => x.ConfirmPassword)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "ConfirmPassword", context));

        RuleFor(x => new { x.Password, x.ConfirmPassword })
            .Custom((x, context) =>
                CommonValidator.PasswordAndConfirmPasswordAreTheSame(x.Password, x.ConfirmPassword, context));

        RuleFor(x => x.Gender)
            .Custom((x, context) => CommonValidator.WhetherTheEnumIsCorrect(x, "Gender", context));

        RuleFor(x => new { x.Password, x.ConfirmPassword })
            .Custom((x, context) => roleCommonValidator.CheckIfDefaultRoleExist(context));
    }
}