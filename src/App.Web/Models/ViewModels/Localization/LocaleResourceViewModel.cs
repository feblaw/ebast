using App.Domain.Models.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Web.Models.ViewModels.Localization
{
    public class LocaleResourceViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Language")]
        public int LanguageId { get; set; }

        [Display(Name = "Resource Name")]
        public string ResourceName { get; set; }

        [Display(Name = "Resource Value")]
        public string ResourceValue { get; set; }

        public string CustomField1 { get; set; }

        public string CustomField2 { get; set; }

        public string CustomField3 { get; set; }
    }

    public class LocaleResourceForm
    {
        [Required]
        [Display(Name = "Name")]
        public string ResourceName { get; set; }

        [Required]
        [Display(Name = "Value")]
        public List<ResourceValue> ResourceValues { get; set; }
    }

    public class ResourceValue
    {
        public Guid? LocaleResourceId { get; set; }

        [Required]
        public int LanguageId { get; set; }

        public string LanguageName { get; set; }

        public string Value { get; set; }
    }

    public static class LocaleResourceExtension
    {
        public static void Update(this LocaleResource model, LocaleResourceForm form)
        {
            model.ResourceName = form.ResourceName;
        }

        public static void Update(this LocaleResource target, LocaleResource source)
        {
            target.LanguageId = source.LanguageId;
            target.ResourceName = source.ResourceName;
            target.ResourceValue = source.ResourceValue;
        }
    }
}
