using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class GenerateLog : BaseModel, IEntity
    {
        public GenerateLog()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        public string By { get; set; }
        public string Subscriber { get; set; }
        public string Product { get; set; }
        public string Ledger { get; set; }
        public DateTime PeriodBegin { get; set; }
        public DateTime GeneratedPeriod { get; set; }
    }
}
