using System;
using System.Collections.Generic;
using App.Data.Repository;
using App.Domain.Models.Core;
using System.Linq;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class EmailArchieveService : BaseService<EmailArchieve, IRepository<EmailArchieve>>, 
        IEmailArchieveService
    {
        public EmailArchieveService(IRepository<EmailArchieve> repository) 
            : base(repository)
        {
        }

        public List<EmailArchieve> GetUnRead(string email)
        {
            return _repository.Table.Where(x => x.IsRead == false && x.Tos.Equals(email) && !string.IsNullOrEmpty(x.Status)).OrderByDescending(x=>x.CreatedAt).ToList();
        }

        public List<EmailArchieve> GetUnsentEmail()
        {
            var emails = _repository
                .Table
                .Where(x => !x.IsSent)
                .ToList();

            return emails;
        }
    }
}
