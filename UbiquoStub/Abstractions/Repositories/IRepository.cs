using System;
using System.Linq.Expressions;

namespace UbiquoStub.Abstractions.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

    Task<TEntity> GetByID(object id);
    Task Insert(TEntity entity);
    Task Delete(object id);
    void Delete(TEntity entityToDelete);
    void Update(TEntity entityToUpdate);

}
