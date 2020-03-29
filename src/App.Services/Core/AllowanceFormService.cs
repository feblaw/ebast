using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class AllowanceFormService : BaseService<AllowanceForm, IRepository<AllowanceForm>>, IAllowanceFormService
    {
        public AllowanceFormService(IRepository<AllowanceForm> repository) : base(repository)
        {
        }
    }
}
