using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Utils
{
    public interface ISmtpOptionsService
    {
        string Server { get; set; }
        int Port { get; set; }
        string User { get; set; }
        string Password { get; set; }
        bool UseSsl { get; set; }
        bool RequiresAunthetication { get; set; }
        string PrefferedEncoding { get; set; }
        string FromEmail { get; set; }
        string FromName { get; set; }

    }
}
