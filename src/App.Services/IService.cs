using AutoMapper;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace App.Services
{
    public interface IService<T> where T : class
    {

        T GetById(params object[] ids);

        IList<T> GetAll(params Expression<Func<T, object>>[] includes);

        IQueryable<T> GetAllQ(params Expression<Func<T, object>>[] includes);

        T Add(T model);

        T Update(T model);

        T Delete(T model);

        void Detach(T model);

        DataTablesResponse GetDataTablesResponse<TDto>(IDataTablesRequest request,
            IMapper mapper,
            string where = null,
            params Expression<Func<T, object>>[] includes);

        DataTablesResponse GetDataTablesResponseByQuery<TDto>(IDataTablesRequest request,
            IMapper mapper,
            IQueryable data);
    }
}
