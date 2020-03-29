using App.Services.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;

namespace App.Helper
{
    public class LocalizationHelper
    {
        private ILanguageService _languageService;
        private ILocaleResourceService _resourceService;
        private IHttpContextAccessor _context;

        public LocalizationHelper(ILanguageService languageService,
            ILocaleResourceService resourceService,
            IHttpContextAccessor context)
        {
            _languageService = languageService;
            _resourceService = resourceService;
            _context = context;
        }

        public string GetLocalized(string name)
        {
            var activeCulture = GetCulture();
            var result = _resourceService.GetResourceValue(name, activeCulture);
            if (string.IsNullOrWhiteSpace(result))
            {
                result = name;
            }

            return result;
        }

        public void SetCulture(string culture)
        {
            _context.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }

        public string GetCulture()
        {
            string culture;

            if (_context.HttpContext.Request.Cookies
                .TryGetValue(CookieRequestCultureProvider.DefaultCookieName, out culture))
                return CookieRequestCultureProvider.ParseCookieValue(culture).UICultures[0];

            return _languageService.GetDefaultLanguage()?.LanguageCulture ?? "en-US";
        }
    }
}
