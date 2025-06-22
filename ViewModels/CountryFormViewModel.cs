using CommunityToolkit.Mvvm.ComponentModel;
using Registers.Classes.Repositories;
using Registers.Classes;
using Registers.ViewModels.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.ViewModels
{
    public partial class CountryFormViewModel : FormViewModel<Country>
    {
        [ObservableProperty] private string? _cName;

        public ObservableCollection<Location> ReferencedLocations { get; }

        public bool HasLocationReferences => ReferencedLocations.Any();

        public ObservableCollection<Country> Countries { get; } =
            new(DataRepository.Instance.Get<Country>().ToList());

        public CountryFormViewModel()
            : base()
        {
            ReferencedLocations = new();

            ReferencedLocations.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(HasLocationReferences));
            };
        }

        public CountryFormViewModel(Country country)
            : base(country)
        {
            ReferencedLocations = new(DataRepository.Instance.Get<Location>().Where(l => l.GetCountry() != null && l.GetCountry().Id == country.Id).ToList());

            ReferencedLocations.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(HasLocationReferences));
            };
        }

        protected override Guid GetIdFromModel(Country model) => model.Id;

        protected override void LoadModelData(Country model)
        {
            CName = model.Name;
        }

        protected override Country CreateModelFromState(Guid id)
        {
            return new()
            {
                Id = id,
                Name = CName
            };
        }
    }
}
