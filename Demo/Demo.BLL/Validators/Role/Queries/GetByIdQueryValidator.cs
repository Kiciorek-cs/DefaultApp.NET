using Demo.BLL.Features.Role.Queries.GetById;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.Role.Queries;

public class GetByIdQueryValidator : CustomValidator<GetByIdQuery>
{
    public GetByIdQueryValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var roleCommonValidator = new RoleCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => roleCommonValidator.RoleExistById(x, context));
    }
}