using Demo.BLL.Features.User.Queries.GetById;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.User.Queries;

public class GetByIdQueryValidator : CustomValidator<GetByIdQuery>
{
    public GetByIdQueryValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => userCommonValidator.UserExistById(x, context));
    }
}