using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Models.Localization
{
    public class Language : BaseModel
    {
        #region Ctor

        public Language()
        {

        }

        public Language(string name, string languageCulture, string uniqueSeoCode, string flag)
            : this()
        {
            Name = name;
            LanguageCulture = languageCulture;
            UniqueSeoCode = uniqueSeoCode;
            Flag = flag;
        }

        public Language(string name,
            string languageCulture,
            string uniqueSeoCode,
            string flag,
            DateTime createDate)
            : this(name, languageCulture, uniqueSeoCode, flag)
        {
            CreatedAt = createDate;
        }

        public Language(string name,
            string languageCulture,
            string uniqueSeoCode,
            string flag,
            DateTime createDate,
            string creator)
            : this(name, languageCulture, uniqueSeoCode, flag, createDate)
        {
            CreatedBy = creator;
        }

        public Language(string name, 
            string languageCulture, 
            string uniqueSeoCode, 
            string flag, 
            bool defaultLang)
            : this(name, languageCulture, uniqueSeoCode, flag)
        {
            DefaultLanguage = defaultLang;
        }

        public Language(string name,
            string languageCulture,
            string uniqueSeoCode,
            string flag,
            bool defaultLang,
            DateTime createDate)
            : this(name, languageCulture, uniqueSeoCode, flag, defaultLang)
        {
            CreatedAt = createDate;
        }

        public Language(string name,
            string languageCulture,
            string uniqueSeoCode,
            string flag,
            bool defaultLang,
            DateTime createDate,
            string creator)
            : this(name, languageCulture, uniqueSeoCode, flag, defaultLang, createDate)
        {
            CreatedBy = creator;
        }

        #endregion

        #region Props

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LanguageCulture { get; set; }

        [Required]
        public string UniqueSeoCode { get; set; }

        [Required]
        public string Flag { get; set; }

        public int Order { get; set; }

        public bool DefaultLanguage { get; set; }

        #endregion
    }
}
