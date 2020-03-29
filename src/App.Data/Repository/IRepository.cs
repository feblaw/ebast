using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace App.Data.Repository
{
    public interface IRepository<T>
        where T : class
    {
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        T GetSingle(params object[] ids);
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        void Detach(T entity);
        void Update(T entity);
        void Update(IEnumerable<T> entities);
        void Save();
        void ExecuteQuery(string query);
        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }
    }
}
