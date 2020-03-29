using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class TacticalResourceDto
    {
        public Guid Id { get; set; }

        public int Approved { get; set; }

        public int Forecast { get; set; }

        public int CountSrf { get; set; }

        public DateTime? DateSrf { get; set; }

        public String OtherInfo { get; set; }

        public string CustomField1 { get; set; }

        public DepartementDto Departement { get; set; }
        public DepartementSubDto DepartementSub { get; set; }
    }
}
