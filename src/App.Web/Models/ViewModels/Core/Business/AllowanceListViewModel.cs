using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class AllowanceListViewModel
    {
        public Guid Id { get; set; }
        public string AllowanceNote { get; set; }
        public decimal OnCallNormal { get; set; }
        public decimal ShiftNormal { get; set; }
        public decimal OnCallHoliday { get; set; }
        public decimal ShiftHoliday { get; set; }
        public decimal GrantedHoliday14 { get; set; }
        public Status AllowanceStatus { get; set; }
        public string DataToken { get; set; }

        public PackageTypes Type { get; set; }
        public Guid ServicePackCategoryId { get; set; }
        public Guid ServicePackId { get; set; }
        public ServicePackViewModel ServicePack { get; set; }
        public ServicePackCategoryViewModel ServicePackCategory { get; set; }
        public string ServicePackCategoryName { get; set; }
        public string ServicePackName { get; set; }
    }

    public class AllowanceListModelForm
    {
        public Status AllowanceStatus { get; set; }
        public string DataToken { get; set; }

        public PackageTypes Type { get; set; }
        public Guid ServicePackCategoryId { get; set; }
        public Guid ServicePackId { get; set; } //SSOW

        public decimal OnCallHoliday { get; set; }
        public decimal ShiftHoliday { get; set; }
        public decimal OnCallNormal { get; set; }
        public decimal ShiftNormal { get; set; }
        public decimal GrantedHoliday14 { get; set; }
        public string AllowanceNote { get; set; }
    }
}
