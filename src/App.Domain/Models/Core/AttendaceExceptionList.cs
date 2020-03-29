using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using App.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Models.Identity;
using Newtonsoft.Json;

namespace App.Domain.Models.Core
{
    public class AttendaceExceptionList : BaseModel, IEntity
    {
        public AttendaceExceptionList()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
        [Key]
        public Guid Id { get; set; }

        public AttendanceExceptionListType Type { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public DateTime AddDate { get; set; }
        public int CreateBy { get; set; }
        public StatusOne StatusOne { get; set; }
        public StatusTwo StatusTwo { get; set; }
        public DateTime ApprovedOneDate { get; set; }
        public DateTime ApprovedTwoDate { get; set; }
        public ActiveStatus RequestStatus { get; set; }
        public string Files { get; set; }
        #region Relationships

        public Guid VacancyId { get; set; }

        [ForeignKey("VacancyId")]
        public virtual VacancyList Vacancy { get; set; }


        public Guid NetworkId { get; set; }

        [ForeignKey("NetworkId")]
        public virtual NetworkNumber Network { get; set; }

        public Guid ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public virtual ActivityCode Activity { get; set; }

        public Guid CostId { get; set; }

        [ForeignKey("CostId")]
        public virtual CostCenter CostCenter { get; set; }

        public Guid LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual City Location { get; set; }

     
        public Guid DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Departement Departement { get; set; }

      
        public Guid DepartmentSubId { get; set; }

        [ForeignKey("DepartmentSubId")]
        public virtual DepartementSub DepartementSub { get; set; }


        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; }


        public Guid AccountNameId { get; set; }

        [ForeignKey("AccountNameId")]
        public virtual AccountName AccountName { get; set; }

        public Guid SubOpsId { get; set; }

        [ForeignKey("SubOpsId")]
        public virtual SubOps SubOps { get; set; }

        public int? ApproverOneId { get; set; }

        [ForeignKey("ApproverOneId")]
        public virtual UserProfile ApproverOne { get; set; }

        public int? ApproverTwoId { get; set; }

        [ForeignKey("ApproverTwoId")]
        public virtual UserProfile ApproverTwo { get; set; }

        public Guid TimeSheetTypeId { get; set; }

        [ForeignKey("TimeSheetTypeId")]
        public virtual TimeSheetType TimeSheetType { get; set; }

        public int? ContractorId { get; set; }

        [ForeignKey("ContractorId")]
        public virtual UserProfile Contractor { get; set; }

        public int? AgencyId { get; set; }
        public int? RemainingDays { get; set; }

        [ForeignKey("AgencyId")]
        public virtual UserProfile Agency { get; set; }

        //public virtual List<AttendanceRecord> Record { get; set; } FF7E2D 7A55C9

        public virtual bool IsAnnual()
        {
            try
            {
                if (TimeSheetType.Type.ToLower().Trim() == "Annual Leave".ToLower().Trim())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        //public virtual String DetailTimeSheet()
        //{
        //    try
        //    {
        //        if (this.Record.Any())
        //        {
        //            var data = this.Record
        //                .Select(x => new { Hours = x.Hours, Dates = x.AttendanceRecordDate.ToString("dd MMMM yyyy"), Days = x.AttendanceRecordDate.ToString("dddd") })
        //                .OrderBy(x => x.Dates)
        //                .ToList();
        //            var json = data.AsEnumerable();
        //            return JsonConvert.SerializeObject(json);
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //    return null;
        //}

        #endregion


    }
}
