using System;

namespace App.Domain.DTO.Core
{
    public class WebSettingDto : BaseDto
    {
        #region Props

        public Guid Id { get; set; }        
        public string Name { get; set; }        
        public string Value { get; set; }

        #endregion
    }
}
