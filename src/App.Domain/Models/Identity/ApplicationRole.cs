using System.Globalization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace App.Domain.Models.Identity
{
    public class ApplicationRole : IdentityRole
    {
        #region Constants

        public const string Administrator = "Administrator";
        public const string User = "User";
        public const string Agency = "Agency";
        public const string LineManager = "Line Manager";
        public const string Sourcing = "Sourcing";
        public const string HeadServiceLine = "Head Service Line";
        public const string HeadOfNonOperation = "Head Of Non Operation";
        public const string HeadOfOperation = "Head Of Operation";
        public const string ServiceLine = "Service Line";
        public const string ProjectManager = "Project Manager";
        //public const string ServiceCoordinator = "Service Coordinator";
        public const string Rpm = "Region Project Manager";
        public const string Contractor = "Contractor";

        #endregion

        #region Props

        public string Description { get; set; }
        public string OtherInfo { get; set; }

        #endregion
    }
}
