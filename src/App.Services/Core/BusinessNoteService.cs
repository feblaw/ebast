using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class BusinessNoteService : BaseService<BusinessNote, IRepository<BusinessNote>>, IBusinessNoteService
    {
        public BusinessNoteService(IRepository<BusinessNote> repository) : base(repository)
        {
        }
    }
}
