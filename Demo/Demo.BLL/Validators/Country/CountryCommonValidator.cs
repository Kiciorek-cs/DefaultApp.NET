using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.Country;

public class CountryCommonValidator
{
    private readonly IDemoUnitOfWork _demoUnitOfWork;

    public CountryCommonValidator(IDemoUnitOfWork demoUnitOfWork)
    {
        _demoUnitOfWork = demoUnitOfWork;
    }

    public bool CountryExistById<T>(int id, ValidationContext<T> context)
    {
        var isExist = _demoUnitOfWork.CountryRepository.GetAsync(x => x.Id == id).GetAwaiter().GetResult() is not null;

        if (!isExist)
        {
            context.AddCustomFailure("Country doesn't exist in system.", ValidationKeys.CountryNotExist);

            return false;
        }

        return true;
    }
}