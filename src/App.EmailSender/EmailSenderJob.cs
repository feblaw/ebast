using App.Domain.Models.Core;
using App.Helper;
using App.Services.Core;
using FluentScheduler;
using System;
using System.Collections.Generic;
using App.Services.Core.Interfaces;

namespace App.EmailSender
{
    public class EmailSenderJob
    {
        private readonly IEmailArchieveService _emailArchieveService;
        private readonly MailingHelper _mailing;
        private readonly ConfigHelper _config;
        private readonly CommonHelper _common;

        public EmailSenderJob(IEmailArchieveService emailArchieveService,
            MailingHelper mailingHelper,
            ConfigHelper configHelper,
            CommonHelper commonHelper)
        {
            _emailArchieveService = emailArchieveService;
            _mailing = mailingHelper;
            _config = configHelper;
            _common = commonHelper;
        }

        public void Execute()
        {
            var emails = _emailArchieveService.GetUnsentEmail();

            Console.WriteLine("Total un-sent email: {0} [{1}]",
                emails.Count,
                _common.ToLongString(DateTime.Now));

            if (emails.Count > 0)
            {
                SendEmails(emails);
            }
        }

        private void SendEmails(List<EmailArchieve> emails)
        {
            foreach(var email in emails)
            {
                Console.WriteLine();
                Console.WriteLine("Sending email [{0}]", email.Id);

                var result = _mailing.SendEmail(email).Result;

                Console.WriteLine("Status : {0}", email.IsSent);
                Console.WriteLine("Try Sending : {0}", email.TrySentCount);
                Console.WriteLine("Last Try Date: {0}", 
                    _common.ToLongString(email.LastTrySentDate));
                Console.WriteLine();
            }
        }
    }
}
