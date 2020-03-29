using App.Data.Repository;
using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Identity;
using Npgsql;

namespace App.Services.Core
{
    public class SrfMigrationService : BaseService<SrfRequest, IRepository<SrfRequest>>, ISrfMigrationService
    {
        private readonly ICandidateInfoService _candidate;
        private readonly IVacancyListService _vacancy;

        public SrfMigrationService(
            IRepository<SrfRequest> repository,
            ICandidateInfoService candidate,
            IVacancyListService vacancy
            ) : base(repository)
        {
            _candidate = candidate;
            _vacancy = vacancy;
        }

        public  bool DeleteAllContractor(string connString)
        {
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(connString);
                conn.Open();
                string sql = "DELETE FROM \"Dummy\" ";
                NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return true;
                }
                conn.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        public AccountName getAccountNameByName(string name)
        {
            throw new NotImplementedException();
        }

        public City getCityByName(string name)
        {
            throw new NotImplementedException();
        }

        public CostCenter getCostCenterByCode(string code)
        {
            throw new NotImplementedException();
        }

        public Departement getDepartementByName(string name)
        {
            throw new NotImplementedException();
        }

        public DepartementSub getDepartementSubByName(string name)
        {
            throw new NotImplementedException();
        }

        public JobStage getJobStageByName(string name)
        {
            throw new NotImplementedException();
        }

        public NetworkNumber getNetworkNumberByNetwork(string code)
        {
            throw new NotImplementedException();
        }

        public PackageType getPackageTypeByName(string name)
        {
            throw new NotImplementedException();
        }

        public SrfRequest getPreviousSrf(DateTime Date)
        {
            throw new NotImplementedException();
        }

        public ServicePack getServicePackByName(string name)
        {
            throw new NotImplementedException();
        }

        public ServicePackCategory getServicePackCategoryByName(string name)
        {
            throw new NotImplementedException();
        }

        public UserProfile getUserProfileByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public  void SetActiveSrf(string connString)
        {

            try
            {
                //NpgsqlConnection conn = new NpgsqlConnection(connString);
                //conn.Open();
                //string sql = "UPDATE \"SrfRequest\" SET \"IsActive\" = FALSE , \"IsLocked\" = true, \"ExtendFrom\" = null, \"AnnualLeave\" = 0";
                //NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                //command.ExecuteReader();
                //conn.Close();

                var DataSrf = _repository.GetAll().Where(x => x.CreatedAt.Value == new DateTime(2011, 11, 11) || x.CreatedAt.Value == DateTime.Now).GroupBy(x => x.CandidateId).Select(x => x.Key).ToList();
                //var DataSrf = _repository.GetAll().GroupBy(x => x.CandidateId).Select(x => x.Key).ToList();
                //var DataSrf = _repository.GetAll().Where(x => x.CandidateId == Guid.Parse("00daec03-7234-430f-b166-0a0bebe602fd")).ToList();
                if (DataSrf != null)
                {
                    int i = 0;
                    foreach (var row in DataSrf)
                    {
                        SetActive(row);
                        i++;
                    }
                    Console.WriteLine(i + " has been actived");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public int setAnnualLeave(DateTime FirstDate, DateTime LastDate)
        {
            throw new NotImplementedException();
        }

        void SetActive(Guid id)
        {
            var SrfParent = _repository.GetAll().Where(x => x.CandidateId.Equals(id)).OrderBy(x=>x.SrfBegin).FirstOrDefault();
            if(SrfParent!=null)
            {
                var SrfExtend = _repository.GetAll().Where(x => x.CandidateId.Equals(id) && (x.SrfBegin > SrfParent.SrfBegin || x.SrfEnd > SrfParent.SrfEnd)).ToList();
                if(SrfExtend!=null)
                {
                    SrfParent.IsActive = false;
                    SrfParent.IsLocked = false;
                    SrfParent.IsExtended = true;
                    SrfParent.AnnualLeave = Distance(SrfParent.SrfBegin.Value, SrfParent.SrfEnd.Value);
                    _repository.Update(SrfParent);

                    Guid ParentId = SrfParent.Id;
                    int Annual = SrfParent.AnnualLeave;
                    foreach(var row in SrfExtend)
                    {
                        var Child = _repository.GetSingle(row.Id);
                        Child.IsExtended = true;
                        Child.ExtendFrom = ParentId;
                        Child.AnnualLeave = Distance(SrfParent.SrfBegin.Value, SrfParent.SrfEnd.Value) + Annual;
                        _repository.Update(Child);
                        ParentId = Child.Id;
                        Annual = Child.AnnualLeave;
                    }

                    var LastActive = _repository.GetAll().Where(x => x.CandidateId.Equals(id)).OrderByDescending(x=> new { x.SrfBegin, x.SrfEnd }).FirstOrDefault();
                    LastActive.IsActive = true;
                    LastActive.IsLocked = false;
                    _repository.Update(LastActive);

                }
                else
                {
                    SrfParent.IsActive = true;
                    SrfParent.IsLocked = false;
                    SrfParent.IsExtended = false;
                    SrfParent.AnnualLeave = Distance(SrfParent.SrfBegin.Value, SrfParent.SrfEnd.Value);
                    _repository.Update(SrfParent);
                }
            }

        }

        int Distance(DateTime first, DateTime last)
        {
            return Math.Abs((first.Month - last.Month) + 12 * (first.Year - last.Year));
        }
    }
}

