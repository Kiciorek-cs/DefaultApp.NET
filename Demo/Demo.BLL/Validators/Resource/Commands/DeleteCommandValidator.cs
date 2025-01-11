using Demo.BLL.Features.Resource.Commands.Delete;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.Resource.Commands;

public class DeleteCommandValidator : CustomValidator<DeleteCommand>
{
    public DeleteCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var resourceCommonValidator = new ResourceCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => resourceCommonValidator.ResourceExistById(x, context));
    }
}