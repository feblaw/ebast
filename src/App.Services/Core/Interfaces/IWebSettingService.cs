using App.Domain.Models.Core;

namespace App.Services.Core.Interfaces
{
    public interface IWebSettingService : IService<WebSetting>
    {
        WebSetting GetByName(string name);
    }
}
