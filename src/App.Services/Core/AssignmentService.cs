using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;
using App.Domain.Models.Identity;
using App.Services.Identity;

namespace App.Services.Core
{
    public class AssignmentService : BaseService<Assignment, IRepository<Assignment>>, IAssignmentService
    {
        //private ICandidateInfoService _candidate;
        private IUserProfileService _profile;

        public AssignmentService(IUserProfileService profile, IRepository<Assignment> repository) : base(repository)
        {
            //_candidate = candidate;
            _profile = profile;
        }

       
        
    }
}
