using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO
{
    public class BaseDto
    {
        public DateTime? CreatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public Guid? LastEditedBy { get; set; }

        public string OtherInfo { get; set; }

        public string CustomField1 { get; set; }

        public string CustomField2 { get; set; }

        public string CustomField3 { get; set; }
    }
}
