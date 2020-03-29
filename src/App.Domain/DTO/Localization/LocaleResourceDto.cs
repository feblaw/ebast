using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Localization
{
    public class LocaleResourceDto : BaseDto
    {
        #region Props

        public Guid Id { get; set; }
        
        public int LanguageId { get; set; }
        
        public string ResourceName { get; set; }
        
        public string ResourceValue { get; set; }

        public LanguageDto Language { get; set; }

        #endregion
    }
}
