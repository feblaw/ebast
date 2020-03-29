using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Models.Core
{
    public class CustomerData : BaseModel, IEntity
    {
        public CustomerData()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Add { get; set; }
        public string Picture { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public CustomerType CustomerType { get; set; }
        public string Email { get; set; }
        //public string LoginId { get; set; }
        //public string LoginPassword { get; set; }
        public bool IsSparepartSupplied { get; set; }
        public bool IsAntarJemput { get; set; }
        public string Warranty { get; set; }
        public bool IsOwnRisk { get; set; }
        public int Times { get; set; }
        public bool IsCommissioned { get; set; }
        public decimal CommissionValue { get; set; }
        public string MontlyCommissionTargetName { get; set; }
        public string CommissionPaymentType { get; set; }
        public string BankName { get; set; }
        public string BrokenCommissionTargetName { get; set; } //nama tujuan komisi putus
        public string KPTSBankName { get; set; }
        public string KPTSRekNumber { get; set; }
        public decimal CommissionTotal  { get; set; }
        public string ClaimDocument { get; set; }
        public string ClaimProcess { get; set; }
        public bool IsWaitingSpk { get; set; }//status kerja unit
        public string Branding { get; set; } //merk dagang
        public string Token { get; set; }
        public char StatusCustomer { get; set; }



        #region relationship

        //public int InsuranceId { get; set; }

        public Guid BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual SystemBranch Branch { get; set; }

        #endregion
    }
}
