using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.Models.Core
{
    public class BackupLog : BaseModel, IEntity
    {
        public BackupLog()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        public DateTime BackupDate { get; set; }
        public string Database { get; set; }
        public string Tables { get; set; }
        public string Succeed { get; set; }
        public string Failed { get; set; }
        public Cycle Cycle { get; set; }
    }
}
