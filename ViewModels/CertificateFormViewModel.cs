using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Registers.Classes;
using Registers.Classes.Helpers;
using Registers.Classes.Repositories;
using Registers.ViewModels.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Registers.ViewModels
{
    public partial class CertificateFormViewModel : FormViewModel<Certificate>
    {
        [ObservableProperty] private CertificateType _cType;
        [ObservableProperty] private DateTime _cDate = DateTime.Now;
        [ObservableProperty] private string? _cUrl;
        [ObservableProperty] private int _cNumber;

        [ObservableProperty] private string? _locationInput;

        public ObservableCollection<Location> Locations { get; } =
            new(DataRepository.Instance.Get<Location>().ToList());

        public ObservableCollection<CertificateType> CertificateTypes { get; } =
            new((CertificateType[])Enum.GetValues(typeof(CertificateType)));

        public CertificateFormViewModel()
            : base() {}

        public CertificateFormViewModel(Certificate certificate)
            : base(certificate) {}

        protected override Guid GetIdFromModel(Certificate model) => model.Id;

        protected override void LoadModelData(Certificate model)
        {
            CType = model.Type;
            CDate = model.Date;
            LocationInput = model.GetLocation()?.ToString();
            CUrl = model.URL;
            CNumber = model.CertificateNumber;
        }

        protected override Certificate CreateModelFromState(Guid id)
        {
            Location? location = ProcessLocationInput();

            return new()
            {
                Id = id,
                Type = CType,
                Date = CDate,
                Location = location?.Id ?? Guid.Empty,
                URL = CUrl,
                CertificateNumber = CNumber
            };
        }

        // checks if the country already exists, if not it creates a new one
        // if no country is given, it returns null
        private Guid? GetCountryId(string? country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return null;

            var countries = DataRepository.Instance.Get<Country>().ToList();
            bool countryExists = countries.Any(l => Helper.ContainsIgnoreCaseAndDiacritics(l.Name, country));


            Country? c = null;
            if (!countryExists)
            {
                var newCountry = new Country(country);
                c = newCountry;
                DataRepository.Instance.Add(newCountry);
            }
            else
            {
                c = countries.FirstOrDefault(l => Helper.ContainsIgnoreCaseAndDiacritics(l.Name, country));
            }

            return c?.Id ?? null;
        }

        private Location ProcessLocationInput()
        {
            Location? location = null;
            if (!string.IsNullOrWhiteSpace(LocationInput))
            {
                var locationAndCountry = LocationInput.Split(',')
                    .Select(s => s.Trim())
                    .ToList();

                var city = locationAndCountry.ElementAtOrDefault(0) ?? string.Empty;
                var countryName = locationAndCountry.ElementAtOrDefault(1) ?? string.Empty;
                var countryId = GetCountryId(countryName);

                location = Locations.FirstOrDefault(l =>
                    Helper.ContainsIgnoreCaseAndDiacritics(l.City, city) &&
                    l.CountryId == countryId);

                if (location == null)
                {
                    // No exact match, check if city exists at all
                    bool cityExists = Locations.Any(l =>
                        Helper.ContainsIgnoreCaseAndDiacritics(l.City, city));

                    // Create new location if needed
                    location = CreateNewLocation(city, countryName);
                }
            }
            return location;
        }

        private Location CreateNewLocation(string city, string country)
        {
            var newLocation = new Location(city, GetCountryId(country));

            Locations.Add(newLocation);
            DataRepository.Instance.Add(newLocation);

            return newLocation;
        }
    }
}
