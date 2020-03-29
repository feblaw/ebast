using App.Domain.DTO.Identity;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class RegistrationListDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public VacanStatusFirst StatusOne { get; set; }

        public VacanStatusSecond StatusTwo { get; set; }

        public VacanStatusThirth StatusThree { get; set; }

        public VacanStatusFourth StatusFourth { get; set; }

        public DateTime DateApprovedOne { get; set; }

        public DateTime DateApprovedTwo { get; set; }

        public DateTime DateApprovedThree { get; set; }

        public DateTime DateApprovedFour { get; set; }

        public DateTime JoinDate { get; set; }

        public VacanStatusFive Status { get; set; }

        public int OtLevel { get; set; }

        public bool isLaptop { get; set; }

        public bool isUsim { get; set; }

        public decimal NoarmalRate { get; set; }

        public bool isManager { get; set; }

        public bool isHrms { get; set; }

        public string Files { get; set; }

        public ApproverStatus RegistrationStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        #region additional
        public String RequestBy { get; set; }
        public String ServicePackCategory { get; set; }
        public String ServicePack { get; set; }
        public int CountCandidate { get; set; }

        public int? ApproverOneId { get; set; }
        public int? ApproverTwoId { get; set; }
        #endregion

    }
}
