using App.Domain.Models.Localization;
using System.ComponentModel.DataAnnotations;

namespace App.Web.Models.ViewModels.Localization
{
    public class LanguageViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Language Culture")]
        public string LanguageCulture { get; set; }

        [Display(Name = "Unique Seo Code")]
        public string UniqueSeoCode { get; set; }

        public string Flag { get; set; }

        [Display(Name = "Display Order")]
        public int Order { get; set; }

        [Display(Name = "Is Default Language")]
        public bool DefaultLanguage { get; set; }
    }

    public class LanguageForm
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Language Culture")]
        public string LanguageCulture { get; set; }

        [Required]
        [Display(Name = "Unique Seo Code")]
        public string UniqueSeoCode { get; set; }

        [Required]
        [UIHint("SingleFileUploadCropper")]
        public string Flag { get; set; }

        [Display(Name = "Display Order")]
        public int Order { get; set; }

        [Display(Name = "Is Default Language")]
        public bool DefaultLanguage { get; set; }
    }

    public static class LanguageExtension
    {
        public static void Update(this Language language, LanguageForm form)
        {
            language.Name = form.Name;
            language.LanguageCulture = form.LanguageCulture;
            language.UniqueSeoCode = form.UniqueSeoCode;
            language.Flag = form.Flag;
            language.Order = form.Order;
            language.DefaultLanguage = form.DefaultLanguage;
        }
    }
}
