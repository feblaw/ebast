using App.Data.Repository;
using App.Domain.Models.Core;
using System.Linq;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class WebSettingService : BaseService<WebSetting, IRepository<WebSetting>>, IWebSettingService
    {
        public WebSettingService(IRepository<WebSetting> repository) 
            : base(repository)
        {
        }

        public WebSetting GetByName(string name)
        {
            return _repository
                .Table
                .Where(x => x.Name == name)
                .SingleOrDefault();
        }
    }
}
