using Demo.BLL.Features.User.Commands.Delete;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class DeleteCommandValidator : CustomValidator<DeleteCommand>
{
    public DeleteCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => userCommonValidator.UserExistById(x, context));
    }
}