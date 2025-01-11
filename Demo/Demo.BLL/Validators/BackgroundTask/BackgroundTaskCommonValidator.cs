using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.BackgroundTask;

public class BackgroundTaskCommonValidator
{
    private readonly IDemoUnitOfWork _demoUnitOfWork;

    public BackgroundTaskCommonValidator(IDemoUnitOfWork demoUnitOfWork)
    {
        _demoUnitOfWork = demoUnitOfWork;
    }

    public bool BackgroundTaskExistByName<T>(string text, ValidationContext<T> context)
    {
        var backgroundTask = _demoUnitOfWork.BackgroundTaskRepository.GetAsync(x => x.Task == text).GetAwaiter()
            .GetResult();

        if (backgroundTask is null)
        {
            context.AddCustomFailure($"Background task doesn't exist: {text}.", ValidationKeys.BackgroundTaskNotExist);
            return false;
        }

        return true;
    }
}