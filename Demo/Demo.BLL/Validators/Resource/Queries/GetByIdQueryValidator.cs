using Demo.BLL.Features.Resource.Queries.GetById;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.Resource.Queries;

public class GetByIdQueryValidator : CustomValidator<GetByIdQuery>
{
    public GetByIdQueryValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var resourceCommonValidator = new ResourceCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => resourceCommonValidator.ResourceExistById(x, context));
    }
}