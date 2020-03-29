using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Identity
{
    public class ApplicationRoleDto
    {
        #region Props

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OtherInfo { get; set; }

        #endregion
    }
}
