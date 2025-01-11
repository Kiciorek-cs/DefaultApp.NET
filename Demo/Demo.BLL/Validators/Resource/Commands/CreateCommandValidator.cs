using Demo.BLL.Features.Resource.Commands.Create;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Validators.Role;
using FluentValidation;

namespace Demo.BLL.Validators.Resource.Commands;

public class CreateCommandValidator : CustomValidator<CreateCommand>
{
    public CreateCommandValidator()
    {

        RuleFor(x => x.Name)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Name", context));

        RuleFor(x => x.Description)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Description", context));

        RuleFor(x => x.HttpMethod)
            .Custom((x, context) => CommonValidator.WhetherTheEnumIsCorrect(x, "HttpMethod", context));
    }
}