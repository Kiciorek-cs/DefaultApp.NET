using Demo.BLL.Features.Country.Queries.GetById;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.Country.Queries;

public class GetByIdQueryValidator : CustomValidator<GetByIdQuery>
{
    public GetByIdQueryValidator(IDemoUnitOfWork demoUnitOfWork)
    {
        var countryCommonValidator = new CountryCommonValidator(demoUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => countryCommonValidator.CountryExistById(x, context));
    }
}