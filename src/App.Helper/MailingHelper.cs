using App.Domain.Models.Core;
using App.Services.Core;
using MailKit.Net.Smtp;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace App.Helper
{
    public class MailingHelper
    {
        private readonly ViewRender _viewRender;
        private readonly IEmailArchieveService _emailArchieveService;
        private readonly ConfigHelper _config;
        private readonly CommonHelper _commonHelper;
        //private readonly string _smtpUsername;
        //private readonly string _smtpPassword;
        //private readonly string _smtpServer;
        //private readonly int _smtpPort;
        //private readonly bool _smtpSsl;
        //private readonly bool _smtpRequireAuthentication;
        //private readonly bool _mailerEnabled;
        private readonly IHostingEnvironment _env;

        public MailingHelper(ViewRender viewRender,
            IEmailArchieveService emailArchieveService,
            IHostingEnvironment env,
            ConfigHelper config,
            CommonHelper commonHelper)
        {
            _env = env;
            _viewRender = viewRender;
            _emailArchieveService = emailArchieveService;
            _config = config;
            _commonHelper = commonHelper;
            //_smtpUsername = _config.GetConfig("smtp.username");
            //_smtpPassword = _config.GetConfig("smtp.password");
            //_smtpServer = _config.GetConfig("smtp.server");
            //_smtpPort = _config.GetConfigAsInt("smtp.port");
            //_smtpSsl = _config.GetConfigAsBool("smtp.ssl");
            //_smtpRequireAuthentication = _config.GetConfigAsBool("smtp.requires.authentication");
            //_mailerEnabled = _config.GetConfigAsBool("mailer.enabled");
        }

        public EmailArchieve CreateEmail<TModel>(string from,
            string tos,
            string subject,
            string viewName,
            TModel model,
            Dictionary<string, string> additionalData = null,
            string fromName = null,
            string replyTo = null,
            string replyToName = null,
            string plainMessage = null,
            string cc = null,
            string bcc = null,
            string activity = null,
            string linkTo = null,
            string status = null,
            string attachment = null)
        {
            var messages = _viewRender.Render(viewName, model, additionalData);

            var email = new EmailArchieve(from, tos, subject, messages, linkTo, activity, status, attachment, DateTime.Now)
            {
                FromName = fromName,
                Cc = cc,
                Bcc = bcc,
                ReplyTo = replyTo,
                ReplyToName = replyToName,
                Activity = activity,
                LinkTo = linkTo,
                Status = status,
                Attachment = attachment
            };

            if (string.IsNullOrEmpty(email.FromName))
                email.FromName = _config.GetConfig("smtp.from.name");

            var hasPlainText = !string.IsNullOrWhiteSpace(plainMessage);

            if (hasPlainText)
                email.PlainMessage = plainMessage;

            var result = _emailArchieveService.Add(email);

            return result;
        }

        public async Task<EmailArchieve> SendEmail(EmailArchieve email)
        {
            try
            {
                if (!_config.GetConfigAsBool("mailer.enabled"))
                    throw new InvalidOperationException("Mailer is currently disabled");

                if (string.IsNullOrWhiteSpace(email.Tos))
                    throw new ArgumentException("No to address provided");

                if (string.IsNullOrWhiteSpace(email.From))
                    throw new ArgumentException("no from address provided");

                if (string.IsNullOrWhiteSpace(email.Subject))
                    throw new ArgumentException("no subject provided");

                var hasPlainText = !string.IsNullOrWhiteSpace(email.PlainMessage);
                var hasHtml = !string.IsNullOrWhiteSpace(email.HtmlMessage);
                var fromName = email.FromName ?? "";
                var replyToName = email.ReplyToName ?? "";
                var tos = email.Tos.Split(',');
                var hasCc = !string.IsNullOrWhiteSpace(email.Cc);
                var hasBcc = !string.IsNullOrWhiteSpace(email.Bcc);

                if (!hasPlainText && !hasHtml)
                    throw new ArgumentException("no message provided");

                var m = new MimeMessage();

                m.From.Add(new MailboxAddress(fromName, email.From));

                if (!string.IsNullOrWhiteSpace(email.ReplyTo))
                    m.ReplyTo.Add(new MailboxAddress(replyToName, email.ReplyTo));

                foreach (var to in tos)
                    m.To.Add(new MailboxAddress("", to));

                if (hasCc)
                {
                    var ccs = email.Cc.Split(',');
                    foreach (var cc in ccs)
                        m.Cc.Add(new MailboxAddress("", cc));
                }

                if (hasBcc)
                {
                    var bccs = email.Bcc.Split(',');
                    foreach (var bcc in bccs)
                        m.Bcc.Add(new MailboxAddress("", bcc));
                }

                m.Subject = email.Subject;

                var bodyBuilder = new BodyBuilder();

                if (hasPlainText)
                    bodyBuilder.TextBody = email.PlainMessage;

                if (hasHtml)
                    bodyBuilder.HtmlBody = email.HtmlMessage;

                if (!string.IsNullOrWhiteSpace(email.Attachment))
                {
                    var attachments = JsonConvert.DeserializeObject<List<string>>(email.Attachment);
                    if (attachments != null)
                    {
                        foreach (var row in attachments)
                        {
                            var upload = Path.Combine(_env.WebRootPath, row);
                            bodyBuilder.Attachments.Add(upload);
                        }
                    }

                }

                m.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(
                            _config.GetConfig("smtp.username"),
                            _config.GetConfigAsInt("smtp.port"),
                            _config.GetConfigAsBool("smtp.ssl"))
                        .ConfigureAwait(false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    if (_config.GetConfigAsBool("smtp.requires.authentication"))
                    {
                        await client.AuthenticateAsync(_config.GetConfig("smtp.username"), _config.GetConfig("smtp.password"))
                            .ConfigureAwait(false);
                    }

                    await client.SendAsync(m).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);

                    email.SentDate = DateTime.Now;
                    email.IsSent = true;
                }
            }
            catch (Exception ex)
            {
                email.ExceptionSendingMessage = JsonConvert.SerializeObject(ex);
                email.IsSent = false;
            }
            finally
            {
                email.TrySentCount += 1;
                email.LastTrySentDate = DateTime.Now;
                email.LastUpdateTime = DateTime.Now;
            }

            return _emailArchieveService.Update(email);
        }

        public async Task<EmailArchieve> SendEmail(Guid id, bool resend = false)
        {
            var email = _emailArchieveService.GetById(id);

            if (email == null)
                throw new Exception("Email archieve not found");

            if (email.IsSent && !resend)
                throw new Exception(string.Format("Email already sent at {0}",
                    _commonHelper.ToLongString(email.SentDate.Value)));

            return await SendEmail(email);
        }
    }
}
