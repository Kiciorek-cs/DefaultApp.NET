using System.Collections.Generic;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Enums;
using FluentValidation;

namespace Demo.BLL.Validators.Resource;

public class ResourceCommonValidator
{
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public ResourceCommonValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public bool ResourceExistById<T>(int id, ValidationContext<T> context)
    {
        var isExist =
            _permissionUnitOfWork.ResourceRepository.GetAsync(x => x.Id == id).GetAwaiter().GetResult() is not null;

        if (!isExist)
        {
            context.AddCustomFailure("Resource doesn't exist in system.", ValidationKeys.ResourceNotExist);

            return false;
        }

        return true;
    }

    public bool ResourceExistByNameHttpMethod<T>(string name, HttpMethodType httpMethodType,
        ValidationContext<T> context, int? id = null)
    {
        var resource = _permissionUnitOfWork.ResourceRepository.GetAsync(y =>
                y.Name.ToLower() == name.Trim() &&
                y.HttpMethod == httpMethodType &&
                (
                    id == null || y.Id != id
                )
            ).GetAwaiter()
            .GetResult();

        if (resource is not null)
        {
            context.AddCustomFailure(
                $"You cannot attach a resource that is already in the systems {name}, {httpMethodType}.",
                ValidationKeys.ResourceExist, resource.Name.ToLower() == name ? "Name" : "HttpMethod");

            return false;
        }

        return true;
    }

    public bool ResourceExistByIdList<T>(List<int> ids, ValidationContext<T> context)
    {
        foreach (var id in ids)
        {
            var isExist = false;
            isExist = _permissionUnitOfWork.ResourceRepository.GetAsync(x => x.Id == id)
                .GetAwaiter()
                .GetResult() is not null;

            if (!isExist)
            {
                context.AddCustomFailure($"Resource doesn't exist in system: id - {id}.",
                    ValidationKeys.ResourceNotExist);

                return false;
            }
        }

        return true;
    }
}