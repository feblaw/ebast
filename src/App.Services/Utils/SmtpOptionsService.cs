using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Utils
{
    public class SmtpOptionsService :ISmtpOptionsService
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public bool RequiresAunthetication { get; set; }
        public string PrefferedEncoding { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
