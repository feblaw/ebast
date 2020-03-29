using System;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Models.Core
{
    public class EmailArchieve : BaseModel, IEntity
    {
        #region Ctor

        public EmailArchieve()
        {
            Id = Guid.NewGuid();
        }

        public EmailArchieve(string from, string tos, string subject, string message, string link, string activity, string status,string attachment)
            : this()
        {
            From = from;
            Tos = tos;
            Subject = subject;
            HtmlMessage = message;
            LinkTo = link;
            activity = Activity;
            Status = status;
            Attachment = attachment;
            IsRead = false;
        }

        public EmailArchieve(string from,
            string tos,
            string subject,
            string message,
            string link,
            string activity,
            string status,
            string attachment,
            DateTime createdDate)
            : this(from, tos, subject, message, link, activity, status, attachment)
        {
            CreatedAt = createdDate;
        }

        public EmailArchieve(string from,
            string tos,
            string subject,
            string message,
            string link,
            string activity,
            string status,
             string attachment,
            DateTime createdDate,
            string creator)
            : this(from, tos, subject, message, link, activity, status, attachment, createdDate)
        {
            CreatedBy = creator;
        }

        #endregion

        #region Props

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string From { get; set; }

        public string FromName { get; set; }

        [Required]
        public string Tos { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string ReplyTo { get; set; }

        public string ReplyToName { get; set; }

        [Required]
        public string Subject { get; set; }

        public string HtmlMessage { get; set; }

        public string PlainMessage { get; set; }

        public bool IsSent { get; set; }

        public DateTime? SentDate { get; set; }

        public string ExceptionSendingMessage { get; set; }

        public int TrySentCount { get; set; }

        public DateTime? LastTrySentDate { get; set; }

        public string LinkTo { get; set; }
        public string Activity { get; set; }
        public string Status { get; set; }
        public string Attachment { get; set; }
        public bool IsRead { get; set; }

        #endregion
    }
}
