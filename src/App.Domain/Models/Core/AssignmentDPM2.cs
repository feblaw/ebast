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
    public class AssignmentDPM2 : BaseModel, IEntity
    {

        public AssignmentDPM2()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }
        public string _id { get; set; }
        public string Request_Type { get; set; }
        public string ASP_Acceptance_Date { get; set; }
        public string Info_Area_XL { get; set; }
        public string id_project_doc { get; set; }
        public string SOW_Type { get; set; }
        public string Plant { get; set; }
        public string NW_Activity { get; set; }
        public string Requisitioner { get; set; }
        public string Short_Text { get; set; }
        public string Vendor_Name { get; set; }
        public string Vendor_Email { get; set; }
        public string Head_Text { get; set; }
        public string Long_Text { get; set; }
        public string Payment_Terms { get; set; }
        public string Approval_L1 { get; set; }
        public string Approval_L2 { get; set; }
        public string PR_for_ASP { get; set; }
        public string PO_Number { get; set; }
        public string PO_Item { get; set; }
        public string Error_Message { get; set; }
        public string Item_Status { get; set; }
        public string Work_Status { get; set; }
        public string PO_Qty { get; set; }
        public string _etag { get; set; }
        public string Project { get; set; }
        public string Account_Name { get; set; }
        public string Vendor_Code { get; set; }
        public string Site_ID { get; set; }
        public string Site_Name { get; set; }
        public string NW { get; set; }
        public string Assignment_No { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
        public string Vendor_Code_Actual { get; set; }
        public string PO_Date { get; set; }
        public decimal Total_Service_Price { get; set; }
        public bool PR_PO_Cancelation { get; set; }
        public string WP_ID { get; set; }
        public string PR_For_ASP_Date { get; set; }
        //public DateTime CreatedAt { get; set; }







        //public DateTime PRDateCreated { get; set; }

        //public DateTime PODate { get; set; }

        //public bool AssignmentReady4Bast { get; set; }
        //public string ShortTextPO { get; set; }
        //public decimal ValueAssignment  { get; set; }
        //public string SHID { get; set; }


        //public string AssignmentCreateBy { get; set; }
        //public bool AssignmentCancel { get; set; }


        //#region relationship
        //public Guid AspId { get; set; }
        //[ForeignKey("AspId")]
        //public virtual ASP Asp{ get; set; }

        //#endregion
    }




}
