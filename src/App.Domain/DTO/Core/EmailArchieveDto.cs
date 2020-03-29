using System;

namespace App.Domain.DTO.Core
{
    public class EmailArchieveDto : BaseDto
    {
        #region Props
        
        public Guid Id { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string Tos { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string ReplyTo { get; set; }
        public string ReplyToName { get; set; }
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
        public bool IsRead { get; set; }

        #endregion
    }
}
