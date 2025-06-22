using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmCross.Plugin.Messenger;
using Registers.Classes.Repositories;
using Registers.Classes;
using Registers.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registers.ViewModels.Templates;
using System.Windows.Controls;
using System.Runtime.ConstrainedExecution;
using Registers.Classes.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Registers.ViewModels
{
    public partial class LocationViewModel : DataViewModel<Location, LocationFormViewModel>
    {
        public LocationViewModel(IMvxMessenger messenger) : base(messenger) { }

        protected override LocationFormViewModel CreateFormViewModel(Location? existingItem = null)
            => existingItem != null ? new LocationFormViewModel(existingItem) : new LocationFormViewModel();

        protected override System.Windows.Controls.UserControl CreateMessagePayload(LocationFormViewModel formVM)
            => new EditLocation { DataContext = formVM };

        protected override Func<Location, bool> GetFilter()
        {
            var countries = DataRepository.Instance.Get<Country>().ToList();
            var separators = new[] { ' ', ',', '/' };
            var words = SearchText?
                .Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? Array.Empty<string>();

            return loc =>
            {
                if (words.Length == 0)
                    return true;

                var city = loc.City ?? string.Empty;
                var country = countries.FirstOrDefault(c => c.Id == loc.CountryId)?.Name ?? string.Empty;
                var fields = new[] { city, country };

                return MatchAllWords
                    ? Helper.ContainsAllWordsInAnyField(fields, words)
                    : words.Any(word =>
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(city, new[] { word }) ||
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(country, new[] { word }));
            };
        }
    }
}
