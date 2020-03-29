using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Core.Interfaces
{
    public interface ISrfMigrationService : IService<SrfRequest>
    {
        bool DeleteAllContractor(string connString);
        void SetActiveSrf(string connString);
        SrfRequest getPreviousSrf(DateTime Date);
        ServicePackCategory getServicePackCategoryByName(string name);
        ServicePack getServicePackByName(string name);
        Departement getDepartementByName(string name);
        DepartementSub getDepartementSubByName(string name);
        CostCenter getCostCenterByCode(string code);
        NetworkNumber getNetworkNumberByNetwork(string code);
        AccountName getAccountNameByName(string name);
        JobStage getJobStageByName(string name);
        PackageType getPackageTypeByName(string name);
        City getCityByName(string name);
        UserProfile getUserProfileByEmail(string email);
        int setAnnualLeave(DateTime FirstDate, DateTime LastDate);
    }
}
