using App.Domain.Models.Localization;
using System.Collections.Generic;

namespace App.Services.Localization
{
    public interface ILocaleResourceService : IService<LocaleResource>
    {
        string GetResourceValue(string resourceName, string culture);
        List<LocaleResource> GetResources(string resourceName);
    }
}
