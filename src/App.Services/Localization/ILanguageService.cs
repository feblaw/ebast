using App.Domain.Models.Localization;

namespace App.Services.Localization
{
    public interface ILanguageService : IService<Language>
    {
        Language GetDefaultLanguage();

        Language GetByName(string name);

        Language GetBySeoName(string seoName);

        Language GetByCulture(string culture);
    }
}
