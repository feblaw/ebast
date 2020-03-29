using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Models.Localization
{
    public class LocaleResource : BaseModel, IEntity
    {
        #region Ctor

        public LocaleResource()
        {
            Id = Guid.NewGuid();
        }

        public LocaleResource(int langId, string name, string value)
            : this()
        {
            LanguageId = langId;
            ResourceName = name;
            ResourceValue = value;
        }

        public LocaleResource(int langId, string name, string value, DateTime createDate)
            : this(langId, name, value)
        {
            CreatedAt = createDate;
        }

        public LocaleResource(int langId, string name, string value, DateTime createDate, string creator)
            : this(langId, name, value, createDate)
        {
            CreatedBy = creator;
        }

        #endregion

        #region Props

        [Key]
        public Guid Id { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [Required]
        public string ResourceName { get; set; }

        [Required]
        public string ResourceValue { get; set; }

        #endregion

        #region Relationships

        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }

        #endregion
    }
}
