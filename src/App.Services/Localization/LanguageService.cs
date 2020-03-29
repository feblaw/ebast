using App.Data.Repository;
using App.Domain.Models.Localization;
using System.Linq;

namespace App.Services.Localization
{
    public class LanguageService : BaseService<Language, IRepository<Language>>, ILanguageService
    {
        public LanguageService(IRepository<Language> repository) 
            : base(repository)
        {
        }

        public Language GetDefaultLanguage()
        {
            return _repository
                .Table
                .Where(x => x.DefaultLanguage)
                .SingleOrDefault();
        }

        public Language GetByName(string name)
        {
            return _repository
                .Table
                .Where(x => x.Name == name)
                .SingleOrDefault();
        }

        public Language GetBySeoName(string seoName)
        {
            return _repository
                .Table
                .Where(x => x.UniqueSeoCode == seoName)
                .SingleOrDefault();
        }

        public Language GetByCulture(string culture)
        {
            return _repository
                .Table
                .Where(x => x.LanguageCulture == culture)
                .SingleOrDefault();
        }

        public override Language Add(Language model)
        {
            if (model.DefaultLanguage)
            {
                var others = _repository
                    .GetAll()
                    .Where(x => x.DefaultLanguage)
                    .ToList();

                foreach (var other in others)
                {
                    other.DefaultLanguage = false;
                    Update(other);
                }
            }

            return base.Add(model);
        }

        public override Language Update(Language model)
        {
            if (model.DefaultLanguage)
            {
                var others = _repository
                    .GetAll()
                    .Where(x => x.DefaultLanguage && x.Id != model.Id)
                    .ToList();

                foreach (var other in others)
                {
                    other.DefaultLanguage = false;
                    Update(other);
                }
            }

            return base.Update(model);
        }
    }
}
