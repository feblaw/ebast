using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class ProjectsService : BaseService<Projects, IRepository<Projects>>, IProjectsService
    {
        public ProjectsService(IRepository<Projects> repository) : base(repository)
        {
        }
    }
}
