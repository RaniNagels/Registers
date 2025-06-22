using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Registers.Classes;
using Registers.Classes.Repositories;
using Registers.ViewModels.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;

namespace Registers.ViewModels
{
    public partial class LocationFormViewModel : FormViewModel<Location>
    {
        [ObservableProperty] private string? _cCity;
        [ObservableProperty] private string? _countryInput;

        public ObservableCollection<Certificate> ReferencedCertificates { get; }

        public bool HasReferences => ReferencedCertificates.Any();

        public ObservableCollection<Country> Countries { get; } =
            new(DataRepository.Instance.Get<Country>().ToList());

        public LocationFormViewModel()
            : base() 
        {
            ReferencedCertificates = new();

            ReferencedCertificates.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(HasReferences));
            };
        }

        public LocationFormViewModel(Location location)
            : base(location) 
        { 
            ReferencedCertificates = new(DataRepository.Instance.Get<Certificate>().Where(c => c.GetLocation() == location).ToList());

            ReferencedCertificates.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(HasReferences));
            };
        }

        protected override Guid GetIdFromModel(Location model) => model.Id;

        protected override void LoadModelData(Location model)
        {
            CCity = model.City;
            CountryInput = model.GetCountry()?.Name;
        }

        protected override Location CreateModelFromState(Guid id)
        {
            Country? country = null;
            if (!string.IsNullOrWhiteSpace(CountryInput))
            {
                bool exists = Countries.Any(l =>
                    l.Name?.Equals(CountryInput, StringComparison.OrdinalIgnoreCase) == true);

                if (!exists)
                {
                    var newCountry = new Country(CountryInput); // or choose default country
                    Countries.Add(newCountry);
                    country = newCountry;

                    DataRepository.Instance.Add(newCountry);
                }
                else
                {
                    country = Countries.FirstOrDefault(l =>
                        l.Name?.Equals(CountryInput, StringComparison.OrdinalIgnoreCase) == true);
                }
            }

            return new()
            {
                Id = id,
                City = CCity,
                CountryId = country?.Id ?? Guid.Empty
            };
        }
    }
}
