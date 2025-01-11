using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Enums.Permission;
using FluentValidation;

namespace Demo.BLL.Validators.User;

public class UserCommonValidator
{
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public UserCommonValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public bool UserExistById<T>(int id, ValidationContext<T> context)
    {
        var isExist = _permissionUnitOfWork.UserRepository
            .GetAsync(x => x.Id == id && x.Login.Status != AccountStatusType.Deleted).GetAwaiter()
            .GetResult() is not null;

        if (!isExist)
        {
            context.AddCustomFailure($"User doesn't exist in system: {id}.", ValidationKeys.UserNotExist);

            return false;
        }

        return true;
    }

    public bool UserCanBeConfirmed<T>(int id, ValidationContext<T> context)
    {
        var user = _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == id).GetAwaiter().GetResult();

        if (user.Login.EmailValidationStatus == EmailValidationStatusType.Confirmed)
        {
            context.AddCustomFailure($"The account has been activated: {user.Login.LoginName}.",
                ValidationKeys.UserConfirmed);

            return false;
        }

        return true;
    }

    public bool UserExistByUserName<T>(string userName, ValidationContext<T> context, int? id = null)
    {
        var isExist = _permissionUnitOfWork.LoginRepository.GetAsync(x => (id == null || x.Id != id) &&
                                                                          x.LoginName == userName).GetAwaiter()
            .GetResult() is null;

        if (!isExist)
        {
            context.AddCustomFailure($"A user with the given name exists: {userName}.", ValidationKeys.UserExist);

            return false;
        }

        return true;
    }

    public bool UserExistByEmailAddress<T>(string emailAddress, ValidationContext<T> context)
    {
        var isExist = _permissionUnitOfWork.LoginRepository
            .GetAsync(x => x.EmailAddress == emailAddress && x.Status != AccountStatusType.Deleted).GetAwaiter()
            .GetResult() is not null;

        if (!isExist)
        {
            context.AddCustomFailure($"User doesn't exist in system: {emailAddress}.", ValidationKeys.UserNotExist);

            return false;
        }

        return true;
    }

    public bool UserNotExistByEmailAddress<T>(string emailAddress, ValidationContext<T> context)
    {
        var isExist = _permissionUnitOfWork.LoginRepository.GetAsync(x => x.EmailAddress == emailAddress).GetAwaiter()
            .GetResult() is not null;

        if (isExist)
        {
            context.AddCustomFailure($"A user with the given email exists: {emailAddress}.", ValidationKeys.UserExist);

            return false;
        }

        return true;
    }

    public bool WhetherUserHaveActiveAccount<T>(string emailAddress, ValidationContext<T> context)
    {
        var user = _permissionUnitOfWork.LoginRepository.GetAsync(x => x.EmailAddress == emailAddress).GetAwaiter()
            .GetResult();

        if (user.EmailValidationStatus == EmailValidationStatusType.NotConfirmed)
        {
            context.AddCustomFailure("The user account is not confirmed.", ValidationKeys.UserSuspended);

            return false;
        }

        if (user.Status == AccountStatusType.Suspended)
        {
            context.AddCustomFailure("The user account has been suspended.", ValidationKeys.UserSuspended);

            return false;
        }

        if (user.Status == AccountStatusType.Deleted)
        {
            context.AddCustomFailure("The user account has been deleted.", ValidationKeys.UserDeleted);

            return false;
        }

        return true;
    }

    public bool CheckIfUserIsInCurrentState<T>(int id, AccountStatusType accountStatusType,
        ValidationContext<T> context)
    {
        var login = _permissionUnitOfWork.LoginRepository.GetAsync(x => x.UserId == id).GetAwaiter().GetResult();

        if (login.Status == AccountStatusType.Active && accountStatusType == AccountStatusType.Active)
        {
            context.AddCustomFailure("The user is active.", ValidationKeys.UserActive, "Id");

            return false;
        }

        if (login.Status == AccountStatusType.Suspended && accountStatusType == AccountStatusType.Suspended)
        {
            context.AddCustomFailure("The user is suspended.", ValidationKeys.UserSuspend, "Id");

            return false;
        }

        return true;
    }
}