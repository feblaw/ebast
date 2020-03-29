using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.DTO.Core
{
    public class ServicePackDto : BaseDto
    {
        public Guid Id { get; set; }
        public PackageTypes Type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ServiceCode { get; set; }
        public decimal Rate { get; set; }
        public decimal Hourly { get; set; }
        public decimal Otp20 { get; set; }
        public decimal Otp30 { get; set; }
        public decimal Otp40 { get; set; }
        public decimal Laptop { get; set; }
        public decimal Usin { get; set; }
        public SpacateStatus Status { get; set; }
        public string Token { get; set; }
        public Guid ServicePackCategoryId { get; set; }
        public ServicePackCategoryDto ServicePackCategory { get; set; }
        public string Package { get; set; }
    }
}
