using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Messages
{
    public interface IMailSenderService
    {
        Task SendEmailAsync(
            string to,
            string from,
            string subject,
            string plainTextMessage,
            string htmlMessage,
            string replyTo = null);

        Task SendEmailAsync(
            string to,
            string subject,
            string message);
    }

    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string messages);
    }

    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
