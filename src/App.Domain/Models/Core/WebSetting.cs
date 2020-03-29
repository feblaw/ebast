using System;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Models.Core
{
    public class WebSetting : BaseModel, IEntity
    {
        #region Ctor

        public WebSetting()
        {
            Id = Guid.NewGuid();
        }

        public WebSetting(string name, string value)
            : this()
        {
            Name = name;
            Value = value;
        }

        public WebSetting(string name, string value, DateTime createDate)
            : this(name, value)
        {
            CreatedAt = createDate;
        }

        public WebSetting(string name, string value, DateTime createDate, string creator)
            : this(name, value, createDate)
        {
            CreatedBy = creator;
        }

        #endregion

        #region Props

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public bool SystemSetting { get; set; }

        #endregion
    }
}
