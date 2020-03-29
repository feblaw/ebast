using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;
using System.Security.Claims;
using App.Domain.Models.Identity;
using App.Domain.DTO.Core;
using System.Globalization;
using App.Domain.Models.Enum;

namespace App.Services.Core
{
    public class AttendaceExceptionListService : BaseService<AttendaceExceptionList, IRepository<AttendaceExceptionList>>, IAttendaceExceptionListService
    {
        private IRepository<NetworkNumber> _network;
        private IRepository<ActivityCode> _activity;
        private IRepository<CostCenter> _costCenter;
        private IRepository<City> _city;
        private IRepository<Departement> _department;
        private IRepository<DepartementSub> _departmentSub;
        private IRepository<Projects> _projects;
        private IRepository<AccountName> _accountName;
        private IRepository<SubOps> _subOpsCode;
        private IRepository<UserProfile> _user;
        private IRepository<TimeSheetType> _timeSheetType;
        private IRepository<AttendanceRecord> _record;

        public AttendaceExceptionListService(
            IRepository<AttendaceExceptionList> repository,
            IRepository<NetworkNumber> network,
            IRepository<ActivityCode> activity,
            IRepository<CostCenter> costCenter,
            IRepository<City> city,
            IRepository<Departement> department,
            IRepository<DepartementSub> departmentSub,
            IRepository<Projects> projects,
            IRepository<AccountName> accountName,
            IRepository<SubOps> subOpsCode,
            IRepository<UserProfile> user,
            IRepository<TimeSheetType> timeSheetType,
            IRepository<AttendanceRecord> record
        ) : base(repository)
        {
            _network = network;
            _activity = activity;
            _costCenter = costCenter;
            _city = city;
            _department = department;
            _departmentSub = departmentSub;
            _projects = projects;
            _accountName = accountName;
            _subOpsCode = subOpsCode;
            _user = user;
            _timeSheetType = timeSheetType;
            _record = record;
        }

        public List<ExportExcelsDto> ExportExcel(int UserId, ClaimsPrincipal role, DateTime FirstDate, DateTime LastDate)
        {
            var data = from ts in _repository.GetAll()
                       join nt in _network.Table on ts.NetworkId equals nt.Id
                       join cr in _user.Table on ts.ContractorId equals cr.Id
                       join pr in _user.Table on ts.ApproverOneId equals pr.Id
                       join lm in _user.Table on ts.ApproverTwoId equals lm.Id
                       join ag in _user.Table on ts.AgencyId equals ag.Id
                       join sub in _subOpsCode.Table on ts.SubOpsId equals sub.Id
                       join act in _activity.Table on ts.ActivityId equals act.Id
                       join cs in _costCenter.Table on ts.CostId equals cs.Id
                       join loc in _city.Table on ts.LocationId equals loc.Id into tempCity from cityTable in tempCity.DefaultIfEmpty()
                       join type in _timeSheetType.Table on ts.TimeSheetTypeId equals type.Id
                       select new ExportExcelsDto()
                       {
                           _firstDate = ts.DateStart,
                           _lasttDate = ts.DateEnd,
                           Id = ts.Id,
                           ProjectManagerId = ts.ApproverOneId.Value,
                           LineManagerId = ts.ApproverTwoId.Value,
                           ContractorId = cr.Id,
                           AgencyId = ag.Id,
                           NetworkCode = !string.IsNullOrWhiteSpace(nt.Code) ? nt.Code : "-",
                           NetworkDescription = !string.IsNullOrWhiteSpace(nt.Description) ? nt.Description : "-",
                           SubOpsCode = !string.IsNullOrWhiteSpace(sub.Code) ? sub.Code : "N/A",
                           SubOpsName = !string.IsNullOrWhiteSpace(sub.Description) ? sub.Description : "-",
                           ActCode = !string.IsNullOrWhiteSpace(act.Code) ? act.Code : "N/A",
                           ActName = !string.IsNullOrWhiteSpace(act.Description) ? act.Description : "-",
                           CosCenterCode = !string.IsNullOrWhiteSpace(cs.Code) ? cs.Code : "-",
                           CosCenterName = !string.IsNullOrWhiteSpace(cs.Description) ? cs.Description : "-",
                           ResourceId = !string.IsNullOrWhiteSpace(cr.AhId) ? cr.AhId : "-",
                           ResourceName = !string.IsNullOrWhiteSpace(cr.Name) ? cr.Name : "-",
                           //ProjectName = !string.IsNullOrWhiteSpace(prj.Name) ? prj.Name : "-",
                           //Organization = !string.IsNullOrWhiteSpace(dept.Name) ? dept.Name : "-",
                           //Suborganization = !string.IsNullOrWhiteSpace(deptSub.SubName) ? deptSub.SubName : "-",
                           Location = cityTable == null ? "" : cityTable.Name,
                           PMStatus = Enum.GetName(typeof(StatusOne), ts.StatusOne).ToString(),
                           ApprooveOneBy = !string.IsNullOrWhiteSpace(pr.Name) ? pr.Name : "-",
                           ApprooveOneDateRemark = ts.StatusOne == StatusOne.Waiting ? "" : ts.ApprovedOneDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                           LMStatus = Enum.GetName(typeof(StatusTwo), ts.StatusTwo).ToString(),
                           ApprooveTwoBy = !string.IsNullOrWhiteSpace(lm.Name) ? lm.Name : "-",
                           ApprooveTwoDateRemark = ts.StatusTwo == StatusTwo.Waiting ? "" : ts.ApprovedTwoDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                           PeriodeStart = ts.DateStart.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                           PeriodeEnd = ts.DateEnd.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                           TimeSheetType = !string.IsNullOrWhiteSpace(type.Type) ? type.Type : "-",

                       };

            var wideLeft = FirstDate.AddDays(-6);
            var wideRight = LastDate.AddDays(6);
            data = data.Where(x => x._firstDate >= wideLeft && x._lasttDate <= wideRight);

            if (role.IsInRole("HR Agency"))
            {
                data = data.Where(x => x.ContractorId == UserId);
            }
            //else if (role.IsInRole("HR Agency"))
            //{
            //    data = data.Where(x => x.AgencyId == UserId);
            //}
            else if (role.IsInRole("Regional Project Manager") || role.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => x.LineManagerId == UserId || x.ProjectManagerId == UserId);
            }

            data.OrderBy(x => x.NetworkCode);

            return data.ToList<ExportExcelsDto>();
        }

    }
}
