using App.Domain.DTO.Identity;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class AssignmentDto
    {
        public Guid Id { get; set; }

        public string ProjectName { get; set; }
        public string SiteID { get; set; }
        public string SiteName { get; set; }
        public string AssignmentId { get; set; }
        public DateTime AssignmentCreateDate { get; set; }
        public DateTime AssignmentAcceptedDate { get; set; }
        public string PRNo { get; set; }
        public DateTime PRDateCreated { get; set; }
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public string LineItemPO { get; set; }
        public bool AssignmentReady4Bast { get; set; }
        public string ShortTextPO { get; set; }
        public decimal ValueAssignment { get; set; }
        public string SHID { get; set; }
        public string TOP { get; set; }
        public string AssignmentCreateBy { get; set; }
        public bool AssignmentCancel { get; set; }
        

        #region additional
        public string ASP { get; set; }

        //public int? AspId { get; set; }

        #endregion

    }
}
