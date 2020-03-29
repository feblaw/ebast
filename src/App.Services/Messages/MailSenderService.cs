using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Utils;
using MailKit.Net.Smtp;
using MimeKit;

namespace App.Services.Messages
{
    public class MailSenderService : IMailSenderService
    {
        private readonly ISmtpOptionsService smtp;

        public MailSenderService(ISmtpOptionsService smtp)
        {
            this.smtp = smtp;

        }

        public async Task SendEmailAsync(string to,
            string from,
            string subject,
            string plainTextMessage,
            string htmlMessage,
            string replyTo = null)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("No to address provided");
            }
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentException("no from address provided");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("no subject provided");
            }
            var hasPlainText = !string.IsNullOrWhiteSpace(plainTextMessage);
            var hasHtml = !string.IsNullOrWhiteSpace(htmlMessage);
            if (!hasPlainText && !hasHtml)
            {
                throw new ArgumentException("no message provided");
            }

            var m = new MimeMessage();

            m.From.Add(new MailboxAddress("", from));
            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                m.ReplyTo.Add(new MailboxAddress("", replyTo));
            }
            m.To.Add(new MailboxAddress("", to));
            m.Subject = subject;

            //m.Importance = MessageImportance.Normal;
            //Header h = new Header(HeaderId.Precedence, "Bulk");
            //m.Headers.Add()

            BodyBuilder bodyBuilder = new BodyBuilder();
            if (hasPlainText)
            {
                bodyBuilder.TextBody = plainTextMessage;
            }

            if (hasHtml)
            {
                bodyBuilder.HtmlBody = htmlMessage;
            }

            m.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                        smtp.Server,
                        smtp.Port,
                        smtp.UseSsl)
                    .ConfigureAwait(false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                if (smtp.RequiresAunthetication)
                {
                    await client.AuthenticateAsync(smtp.User, smtp.Password)
                        .ConfigureAwait(false);
                }

                await client.SendAsync(m).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);

            }
        }

        public async Task SendEmailAsync(string to, string subject, string message)
        {
            await SendEmailAsync(to, smtp.FromEmail, subject, message, message);
        }
    }
}
