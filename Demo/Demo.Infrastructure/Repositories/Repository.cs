using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Demo.Infrastructure.Repositories;

public abstract class Repository<TDbContext, T> : IRepository<T>
    where TDbContext : DbContext
    where T : class
{
    protected readonly TDbContext _dbContext;

    protected Repository(TDbContext context)
    {
        _dbContext = context;
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        return (await _dbContext.AddAsync(entity, cancellationToken)).Entity;
    }

    public async Task<List<T>> AddAsync(List<T> entities, CancellationToken cancellationToken = default)
    {
        var listOfEntities = new List<T>();
        foreach (var entity in entities)
            listOfEntities.Add((await _dbContext.AddAsync(entity, cancellationToken)).Entity);
        return listOfEntities;
    }

    public async Task AddRangeAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddRangeAsync(entity);
    }

    public T Update(T entity)
    {
        var returnValue = _dbContext.Update(entity);
        return returnValue.Entity;
    }


    public void Delete(T entity)
    {
        //var idProperty = entity.GetType().GetProperty("Id").GetValue(entity, null);
        //if (idProperty != null)
        //{
        //    var listForeignKeys = ForeignKeyChecker(idProperty).Result;
        //    if (listForeignKeys.Count > 0)
        //        throw new ForeignKeyException(listForeignKeys);
        //}
        _dbContext.Remove(entity);
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetMultipleAsync(Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default)
    {
        if (predicate is not null)
            return await _dbContext.Set<T>().Where(predicate).ToListAsync(cancellationToken);

        return await _dbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetMultipleNoTrackingAsync(Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
            queryable = queryable.Where(predicate);

        return await queryable.AsQueryable().ToListAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(IDbContextTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        await transaction.RollbackAsync(cancellationToken);
    }
}