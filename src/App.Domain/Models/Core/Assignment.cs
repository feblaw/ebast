using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;

namespace App.Domain.Models.Core
{
    public class Assignment : BaseModel, IEntity
    {

        public Assignment()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        [Key]
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
        public string Sow { get; set; }
        public string AssignmentCreateBy { get; set; }
        public string idDPM { get; set; }
        public string id_project_doc { get; set; }
        public bool AssignmentCancel { get; set; }


        #region relationship
        public Guid AspId { get; set; }
        [ForeignKey("AspId")]
        public virtual ASP Asp { get; set; }

        #endregion
    }




}
