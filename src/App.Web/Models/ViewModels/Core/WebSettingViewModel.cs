using App.Domain.Models.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace App.Web.Models.ViewModels.Core
{
    public class WebSettingViewModel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class WebSettingForm
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }

    public static class WebSettingExtension
    {
        public static void Update(this WebSetting webSetting, WebSettingForm model)
        {
            webSetting.Name = model.Name;
            webSetting.Value = model.Value;
        }
    }
}
