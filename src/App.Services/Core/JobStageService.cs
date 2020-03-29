using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class JobStageService : BaseService<JobStage, IRepository<JobStage>>, IJobStageService
    {

        private readonly ICandidateInfoService _candidate;
        private readonly IVacancyListService _vacancy;

        public JobStageService(
            IRepository<JobStage> repository,
            ICandidateInfoService candidate,
            IVacancyListService vacancy
            ) : base(repository)
        {
            _candidate = candidate;
            _vacancy = vacancy;
        }

        public JobStage GetByActiveUser(int UserId)
        {
            var CandidateInfo = _candidate
                .GetAll()
                .Where(x => x.AccountId == UserId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (CandidateInfo != null)
            {
                var Vacancy = _vacancy.GetById(CandidateInfo.VacancyId);
                return _repository.GetSingle(Vacancy.JobStageId);
            }

            return null;
        }
    }
}
