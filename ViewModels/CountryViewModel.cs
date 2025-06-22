using MvvmCross.Plugin.Messenger;
using Registers.Classes.Helpers;
using Registers.Classes.Repositories;
using Registers.Classes;
using Registers.UserControls;
using Registers.ViewModels.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registers.UserControls.Country;

namespace Registers.ViewModels
{
    public partial class CountryViewModel : DataViewModel<Country, CountryFormViewModel>
    {
        public CountryViewModel(IMvxMessenger messenger) : base(messenger) { }

        protected override CountryFormViewModel CreateFormViewModel(Country? existingItem = null)
            => existingItem != null ? new CountryFormViewModel(existingItem) : new CountryFormViewModel();

        protected override System.Windows.Controls.UserControl CreateMessagePayload(CountryFormViewModel formVM)
            => new EditCountry { DataContext = formVM };


        protected override Func<Country, bool> GetFilter()
        {
            var countries = DataRepository.Instance.Get<Country>().ToList();

            var words = SearchText?
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? Array.Empty<string>();

            return country =>
            {
                if (words.Length == 0)
                    return true;

                var countryName = countries.FirstOrDefault(c => c.Name == country.Name)?.Name ?? string.Empty;
                var fields = new[] { countryName };

                return MatchAllWords
                    ? Helper.ContainsAllWordsInAnyField(fields, words)
                    : words.Any(word =>
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(countryName, new[] { word }));
            };
        }
    }
}
