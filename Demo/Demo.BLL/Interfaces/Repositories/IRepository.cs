using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Demo.BLL.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetMultipleAsync(Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetMultipleNoTrackingAsync(Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<List<T>> AddAsync(List<T> entities, CancellationToken cancellationToken = default);
    Task AddRangeAsync(T entity, CancellationToken cancellationToken = default);

    T Update(T entity);

    void Delete(T entity);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
}