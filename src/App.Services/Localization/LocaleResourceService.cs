using App.Data.Repository;
using App.Domain.Models.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Services.Localization
{
    public class LocaleResourceService : BaseService<LocaleResource, 
        IRepository<LocaleResource>>, 
        ILocaleResourceService
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly string _defaultCulture;

        #endregion

        #region Ctor

        public LocaleResourceService(IRepository<LocaleResource> repository,
            ILanguageService languageService) : base(repository)
        {
            _languageService = languageService;
            _defaultCulture = _languageService.GetDefaultLanguage().LanguageCulture;
        }

        #endregion

        #region Utils

        private bool CheckDuplicate(string resourceName, int languageId)
        {
            return _repository
                .GetAll()
                .Any(x => x.ResourceName.ToLower() == resourceName.ToLower() && 
                    x.LanguageId == languageId);
        }

        #endregion

        #region Methods

        public override LocaleResource Add(LocaleResource model)
        {
            model.ResourceName = model.ResourceName.ToLower();

            if (CheckDuplicate(model.ResourceName, model.LanguageId))
                throw new Exception("Duplicate Resource Name");

            return base.Add(model);
        }

        public override LocaleResource Update(LocaleResource model)
        {
            model.ResourceName = model.ResourceName.ToLower();

            var current = _repository
                .GetSingle(model.Id);

            if (current.ResourceName != model.ResourceName && CheckDuplicate(model.ResourceName, model.LanguageId))
                throw new Exception("Duplicate Resource Name");

            return base.Update(model);
        }

        public string GetResourceValue(string resourceName, string culture)
        {
            var resource = _repository
                .GetAll(x => x.Language)
                .FirstOrDefault(x => x.ResourceName.ToLower() == resourceName.ToLower() && x.Language.LanguageCulture == culture);

            if (resource == null && culture != _defaultCulture)
            {
                resource = _repository
                    .GetAll(x => x.Language)
                    .FirstOrDefault(x => x.ResourceName.ToLower() == resourceName.ToLower() && x.Language.LanguageCulture == _defaultCulture);
            }

            if (resource != null)
                return resource.ResourceValue;

            return "";
        }

        public List<LocaleResource> GetResources(string resourceName)
        {
            return _repository
                .GetAll(x => x.Language)
                .Where(x => x.ResourceName == resourceName)
                .ToList();
        }

        #endregion
    }
}
