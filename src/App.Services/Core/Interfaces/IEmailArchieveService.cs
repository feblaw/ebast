using System.Collections.Generic;
using App.Domain.Models.Core;

namespace App.Services.Core.Interfaces
{
    public interface IEmailArchieveService : IService<EmailArchieve>
    {
        List<EmailArchieve> GetUnsentEmail();

        List<EmailArchieve> GetUnRead(string email);
    }
}
