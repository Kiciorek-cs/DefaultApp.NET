using Demo.BLL.Features.Resource.Commands.Update;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.Resource.Commands;

public class UpdateCommandValidator : CustomValidator<UpdateCommand>
{
    public UpdateCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var resourceCommonValidator = new ResourceCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => resourceCommonValidator.ResourceExistById(x, context));

        RuleFor(x => x.Name)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Name", context));

        RuleFor(x => x.Description)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Description", context));

        RuleFor(x => new { x.Name, x.HttpMethod })
            .Custom((x, context) =>
                resourceCommonValidator.ResourceExistByNameHttpMethod(x.Name, x.HttpMethod, context));

        RuleFor(x => x.HttpMethod)
            .Custom((x, context) => CommonValidator.WhetherTheEnumIsCorrect(x, "HttpMethod", context));
    }
}