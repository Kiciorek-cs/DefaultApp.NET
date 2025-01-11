using Demo.BLL.Features.BackgroundTask.Commands.Delete;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.BackgroundTask.Commands;

public class DeleteCommandValidator : CustomValidator<DeleteCommand>
{
    public DeleteCommandValidator(IDemoUnitOfWork demoUnitOfWork)
    {
        var backgroundTaskCommonValidator = new BackgroundTaskCommonValidator(demoUnitOfWork);

        RuleFor(x => x.Text)
            .Custom((x, context) => backgroundTaskCommonValidator.BackgroundTaskExistByName(x, context));
    }
}