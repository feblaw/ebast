using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class SystemBranch : BaseModel, IEntity
    {

        public SystemBranch()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string Remark { get; set; }

        public string BranchCode { get; set; }

        public string UnitInCode { get; set; }

        public string PoCode { get; set; }

        public string NotaCode { get; set; }

        public string NotaNonSercvice { get; set; }

        public string PhoneOne { get; set; }

        public string PhoneTwo { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string CabangCode { get; set; }

        public string CabangToken { get; set; }

        public bool isHeadOffice { get; set; }

        public bool BranchStatus { get; set; }

        public string Criteria { get; set; }

        public string Guaranty { get; set; }

        public string PkbRemark { get; set; }

        public string GuarantyNota { get; set; }

    }

  

}
