using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmCross.Plugin.Messenger;
using Registers.Classes;
using Registers.Classes.Helpers;
using Registers.Classes.Repositories;
using Registers.UserControls;
using Registers.ViewModels.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Registers.ViewModels
{
    public partial class CertificateViewModel : DataViewModel<Certificate, CertificateFormViewModel>
    {
        public CertificateViewModel(IMvxMessenger messenger) : base(messenger) { }

        protected override CertificateFormViewModel CreateFormViewModel(Certificate? existingItem = null)
            => existingItem != null ? new CertificateFormViewModel(existingItem) : new CertificateFormViewModel();

        protected override System.Windows.Controls.UserControl CreateMessagePayload(CertificateFormViewModel formVM)
            => new AddCertificate { DataContext = formVM };

        protected override Func<Certificate, bool> GetFilter()
        {
            var countries = DataRepository.Instance.Get<Country>().ToList();
            var locations = DataRepository.Instance.Get<Location>().ToList();

            var separators = new[] { ' ', ',', '/' };

            var words = SearchText?
                .Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? Array.Empty<string>();

            return cert =>
            {
                if (words.Length == 0)
                    return true;

                string city = locations.FirstOrDefault(l => l.Id == cert.Location)?.City ?? string.Empty;
                string country = countries.FirstOrDefault(c => c.Id == cert.GetLocation()?.CountryId)?.Name ?? string.Empty;
                string type = cert.Type.ToString();
                string number = cert.CertificateNumber.ToString();
                string year = cert.Date.Year.ToString();
                string month = cert.Date.Month.ToString("D2");
                string monthDutch = new CultureInfo("nl-BE").DateTimeFormat.GetMonthName(cert.Date.Month);
                string monthEnglish = new CultureInfo("en-US").DateTimeFormat.GetMonthName(cert.Date.Month);
                string day = cert.Date.Day.ToString();

                string[] fields = new[] { city, country, type, number, year, month, monthDutch, monthEnglish, day };

                return MatchAllWords
                    ? Helper.ContainsAllWordsInAnyField(fields, words)
                    : words.Any(word =>
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(city         , new[] { word }) ||
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(country      , new[] { word }) ||
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(type         , new[] { word }) ||
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(number       , new[] { word }) ||
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(year         , new[] { word }) ||
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(month        , new[] { word }) ||
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(monthDutch   , new[] { word }) ||
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(monthEnglish , new[] { word }) ||
                        Helper.ContainsAnyWordIgnoreCaseAndDiacritics(day          , new[] { word }));  
            };
        }
    }
}
