using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using App.Data.DAL;
using System.Linq.Expressions;

namespace App.Data.Repository
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        #region Fields

        private readonly ApplicationDbContext _context;
        private DbSet<T> _entities;

        #endregion

        #region Ctor

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        public virtual IQueryable<T> GetAll()
        {
            IQueryable<T> query = Entities.AsQueryable();
            return query;
        }

        public virtual IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Entities.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {

            IQueryable<T> query = Entities.Where(predicate);
            return query;
        }

        public virtual T GetSingle(params object[] ids)
        {
            return Entities.Find(ids);
        }

        public virtual void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Add(entity);

            Save();
        }

        public virtual void Add(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            var items = entities.ToList();

            for (int i = 1; i <= items.Count; i++)
            {
                _entities.Add(items[i - 1]);

                if (i % 100 == 0)
                    Save();
            }

            Save();
        }

        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Remove(entity);

            Save();
        }

        public virtual void Delete(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            var items = entities.ToList();

            for (int i = 1; i <= items.Count; i++)
            {
                _entities.Remove(items[i - 1]);

                if (i % 100 == 0)
                    Save();
            }

            Save();
        }

        public virtual void Detach(T entity)
        {
            if (entity == null) return;
            _context.Entry(entity).State = EntityState.Detached;
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _context.Entry(entity).State = EntityState.Modified;

            Save();
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var items = entities.ToList();

            for (int i = 1; i <= items.Count; i++)
            {
                _context.Entry(items[i - 1]).State = EntityState.Modified;

                if (i % 100 == 0)
                    Save();
            }

            Save();
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }

        public void ExecuteQuery(string query)
        {
            _context.Database.ExecuteSqlCommand(query);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table => Entities;

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking => Entities.AsNoTracking();

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }
                return _entities;
            }
            set
            {
                _entities = value;
            }
        }

        #endregion
    }
}
