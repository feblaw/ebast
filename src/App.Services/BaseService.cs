using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using App.Data.Repository;
using System.Linq.Dynamic.Core;
using App.Services.Utils;

namespace App.Services
{

    public class BaseService<T, TRepo> : IService<T>
        where T : class
        where TRepo : IRepository<T>
    {
        protected readonly TRepo _repository;

        public BaseService(TRepo repository)
        {
            _repository = repository;
        }

        public virtual T Add(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _repository.Add(model);

            return model;
        }
        
        public virtual T Update(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _repository.Update(model);

            return model;
        }

        public virtual T Delete(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _repository.Delete(model);

            return model;
        }

        public virtual void Detach(T model)
        {
            if (model == null) return;
            _repository.Detach(model);
        }

        public virtual IList<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            return _repository.GetAll(includes)
                 .ToList();
        }

        public virtual IQueryable<T> GetAllQ(params Expression<Func<T, object>>[] includes)
        {
            return _repository.GetAll(includes);
        }

        public virtual T GetById(params object[] ids)
        {
            return _repository.GetSingle(ids);
        }

        public virtual DataTablesResponse GetDataTablesResponse<TDto>(
            IDataTablesRequest request, 
            IMapper mapper, 
            string where = null, 
            params Expression<Func<T, object>>[] includes)
        {
            var data = _repository.GetAll(includes);

            var searchQuery = request.Search.Value;

            var filteredData = data.AsQueryable();

            if (!string.IsNullOrWhiteSpace(where))
            {
                filteredData = filteredData.Where(where);
            }

            var globalFilter = request.Columns.GetFilter(searchQuery, "OR");

            if (!string.IsNullOrEmpty(globalFilter))
                filteredData = filteredData.Where(globalFilter);

            filteredData = RelationQuery(filteredData, searchQuery);

            var filter = request.Columns.GetFilter();

            if (!string.IsNullOrEmpty(filter))
                filteredData = filteredData.Where(filter);

            var sort = request.Columns.GetSort();

            if (!string.IsNullOrEmpty(sort))
                filteredData = filteredData.OrderBy(sort);

            var dataPage = filteredData.Skip(request.Start).Take(request.Length);

            var response = DataTablesResponse.Create(request,
                data.Count(),
                filteredData.Count(),
                mapper.Map<List<TDto>>(dataPage));

            return response;
        }

        public virtual DataTablesResponse GetDataTablesResponseByQuery<TDto>(
            IDataTablesRequest request, 
            IMapper mapper, 
            IQueryable data)
        {
            var searchQuery = request.Search.Value;

            var filteredData = data.AsQueryable();

            var globalFilter = request.Columns.GetFilter(searchQuery, "OR");

            if (!string.IsNullOrEmpty(globalFilter))
                filteredData = filteredData.Where(globalFilter);

            var filter = request.Columns.GetFilter();

            if (!string.IsNullOrEmpty(filter))
                filteredData = filteredData.Where(filter);

            var sort = request.Columns.GetSort();

            if (!string.IsNullOrEmpty(sort))
                filteredData = filteredData.OrderBy(sort);

            var dataPage = filteredData.Skip(request.Start).Take(request.Length);

            var response = DataTablesResponse.Create(request,
                data.Count(),
                filteredData.Count(),
                mapper.Map<List<TDto>>(dataPage));

            return response;
        }

        protected virtual IQueryable<T> RelationQuery(IQueryable<T> data, string query)
        {
            return data;
        }
    }
}
