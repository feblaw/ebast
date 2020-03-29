using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Localization
{
    public class LanguageDto : BaseDto
    {
        #region Props

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string LanguageCulture { get; set; }
        
        public string UniqueSeoCode { get; set; }
        
        public string Flag { get; set; }

        public int Order { get; set; }

        public bool DefaultLanguage { get; set; }

        #endregion
    }
}
