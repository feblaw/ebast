using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace App.Services.Core
{
    public class AccountNameService : BaseService<AccountName, IRepository<AccountName>>, IAccountNameService
    {
        public AccountNameService(IRepository<AccountName> repository) : base(repository)
        {
        }

    }
}
