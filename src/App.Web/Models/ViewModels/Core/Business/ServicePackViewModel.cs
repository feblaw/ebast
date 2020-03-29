using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    /// <summary>
    /// SSOW View Model
    /// </summary>
    public class ServicePackViewModel
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
        //public string Token { get; set; }
        public Guid ServicePackCategoryId { get; set; }
        public string ServicePackCategoryName { get; set; }
    }
    /// <summary>
    /// SSOW Model Form
    /// </summary>
    public class ServicePackModelForm
    {
        public PackageTypes Type { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string ServiceCode { get; set; }
        public decimal Rate { get; set; }
        public decimal Hourly { get; set; }
        public decimal Otp20 { get; set; }
        public decimal Otp30 { get; set; }
        public decimal Otp40 { get; set; }
        public decimal Laptop { get; set; }
        public decimal Usin { get; set; }
        public SpacateStatus Status { get; set; }
        public Guid ServicePackCategoryId { get; set; }
    }
}
