using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Security.Claims;


namespace App.Helper
{
    public class NotifHelper
    {
        private readonly ConfigHelper _config;
        private readonly MailingHelper _mailingHelper;
        private readonly IUserHelper _userHelper;

        public NotifHelper(
            ConfigHelper config,
            MailingHelper mailingHelper,
            IUserHelper userHelper
        )
        {
            _config = config;
            _mailingHelper = mailingHelper;
            _userHelper = userHelper;
        }

        public void Send(
            ClaimsPrincipal us, // User From
            string Subject, // Subject
            string Mailto,  // User target name
            string UserEmail,  // User target email
            string CallBack, // Link CallBack
            string NotificationTitle, // Email content or descriptions
            string Description, // Description
            NotificationInboxStatus notifStatus, // Notif Status
            Activities activity, // Activity Status,
            string Attachments = null, // Attachment,
            string EmailTemplate = null, // Template,
            Dictionary<string, string> Data = null
        )
        {
            try
            {
                if (!_config.GetConfigAsBool("notif.enabled"))
                    throw new InvalidOperationException("Notification mail is currently disabled");

                var user = _userHelper.GetUser(us);
                if (Data == null)
                {
                    Data = new Dictionary<string, string>()
                  {
                      { "Name", Mailto },
                      { "NotificationTitle", NotificationTitle},
                      { "CallbackUrl", CallBack },
                      { "Description", Description },
                  };
                }

                string Desc = Description;
                if (string.IsNullOrWhiteSpace(Description))
                {
                    Desc = NotificationTitle;
                }

                if (string.IsNullOrEmpty(EmailTemplate))
                {
                    EmailTemplate = "Emails/ActivityEmail";
                }

                var email = _mailingHelper.CreateEmail(
                    _config.GetConfig("smtp.from.email"), //from
                    UserEmail, // Tos
                    Subject, // Subject
                    EmailTemplate, // viewName
                    user, // Model
                    Data, // additionalData
                    null,
                    null, // replyTo
                    null, // replyToName
                    Desc, // plain message
                    null, // cc 
                    null, // bcc
                    Enum.GetName(typeof(Activities), activity),
                    CallBack,
                    Enum.GetName(typeof(NotificationInboxStatus), notifStatus),
                    Attachments
                );

                var emailResult = _mailingHelper.SendEmail(email).Result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
