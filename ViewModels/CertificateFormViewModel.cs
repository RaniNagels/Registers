using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Registers.Classes;
using Registers.Classes.Helpers;
using Registers.Classes.Repositories;
using Registers.ViewModels.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Registers.ViewModels
{
    public partial class CertificateFormViewModel : FormViewModel<Certificate>
    {
        [ObservableProperty] private CertificateType _cType = CertificateType.Birth;
        [ObservableProperty] private DateTime _cDate = DateTime.Now;
        [ObservableProperty] private string? _cUrl;
        [ObservableProperty] private int _cNumber;

        private ObservableCollection<CertificatePersonInfo> _people;
        public ObservableCollection<CertificatePersonInfo> People
        {
            get => _people;
            set
            {
                _people = value;
                OnPropertyChanged();
            }
        }
        private List<CertificatePersonInfo> _storedPeople = new();

        [ObservableProperty] private string? _locationInput;

        // manually set the width of the personinfo listviewitem width
        private double _headerColumnWidth = 150;
        public double HeaderColumnWidth
        {
            get => _headerColumnWidth;
            set
            {
                if (_headerColumnWidth != value)
                {
                    _headerColumnWidth = value;
                    OnPropertyChanged();
                }
            }
        }


        public ObservableCollection<Location> Locations { get; } =
            new(DataRepository.Instance.Get<Location>().ToList());

        public ObservableCollection<CertificateType> CertificateTypes { get; } =
            new((CertificateType[])Enum.GetValues(typeof(CertificateType)));

        public ObservableCollection<RoleInCertificate> AvailableRoles { get; private set; } = 
            new((RoleInCertificate[])Enum.GetValues(typeof(RoleInCertificate)));

        private List<RoleInCertificate> OneTimeRoles = new List<RoleInCertificate>
            {
                RoleInCertificate.Subject,
                RoleInCertificate.Father,
                RoleInCertificate.Mother,
                RoleInCertificate.Spouse,
                RoleInCertificate.Mother_of_the_Groom,
                RoleInCertificate.Father_of_the_Groom,
                RoleInCertificate.Mother_of_the_Bride,
                RoleInCertificate.Father_of_the_Bride,
                RoleInCertificate.Bride,
                RoleInCertificate.Groom
            };
        [ObservableProperty] private RoleInCertificate? _selectedRole;

        public CertificateFormViewModel()
            : base() 
        {
            People = new ObservableCollection<CertificatePersonInfo>();
            SwitchType();
        }

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
            ObservableCollection<CertificatePersonInfo> temp;
            if (model.RawPersonsData != null)
            {
                temp = new ObservableCollection<CertificatePersonInfo>(model.RawPersonsData);
                SetVisibleFields(temp);
            }
            else temp = new ObservableCollection<CertificatePersonInfo>();
            People = temp;

            UpdateAvailableRoles();
        }

        protected override Certificate CreateModelFromState(Guid id)
        {
            Guid? location = GetLocationId(LocationInput);

            return new()
            {
                Id = id,
                Type = CType,
                Date = CDate,
                Location = location ?? Guid.Empty,
                URL = CUrl,
                CertificateNumber = CNumber,
                RawPersonsData = People.ToList()
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

        private Guid? GetLocationId(string? location)
        {
            if (string.IsNullOrWhiteSpace(location)) return Guid.Empty;

            var locationAndCountry = location.Split(',')
                .Select(s => s.Trim())
                .ToList();

            var city = locationAndCountry.ElementAtOrDefault(0) ?? string.Empty;
            var countryName = locationAndCountry.ElementAtOrDefault(1) ?? string.Empty;
            var countryId = GetCountryId(countryName);

            Location? loc = Locations.FirstOrDefault(l =>
                    Helper.ContainsIgnoreCaseAndDiacritics(l.City, city) &&
                    l.CountryId == countryId);

            if (loc == null)
            {
                loc = CreateNewLocation(city, countryName);
            }

            return loc?.Id ?? Guid.Empty;
        }

        private Location CreateNewLocation(string city, string country)
        {
            var newLocation = new Location(city, GetCountryId(country));

            Locations.Add(newLocation);
            DataRepository.Instance.Add(newLocation);

            return newLocation;
        }

        [RelayCommand]
        private void SwitchType()
        {
            foreach (var personInfo in People)
            {
                if (personInfo.HasValue())
                    _storedPeople.Add(personInfo);
            }
            People.Clear();

            List<RoleInCertificate> roles = new List<RoleInCertificate>();
            switch (CType)
            {
                case CertificateType.Birth:
                    roles = new List<RoleInCertificate>
                    {
                        RoleInCertificate.Registrar,
                        RoleInCertificate.Subject,
                        RoleInCertificate.Father,
                        RoleInCertificate.Mother,
                        RoleInCertificate.Witness,
                        RoleInCertificate.Witness
                    };
                    break;
                case CertificateType.Death:
                    roles = new List<RoleInCertificate>
                    {
                        RoleInCertificate.Registrar,
                        RoleInCertificate.Subject,
                        RoleInCertificate.Other,
                        RoleInCertificate.Mother,
                        RoleInCertificate.Father,
                        RoleInCertificate.Spouse,
                        RoleInCertificate.Witness,
                        RoleInCertificate.Witness
                    };
                    break;
                case CertificateType.Marriage:
                    roles = new List<RoleInCertificate>
                    {
                        RoleInCertificate.Registrar,
                        RoleInCertificate.Bride,
                        RoleInCertificate.Groom,
                        RoleInCertificate.Mother_of_the_Bride,
                        RoleInCertificate.Father_of_the_Bride,
                        RoleInCertificate.Mother_of_the_Groom,
                        RoleInCertificate.Father_of_the_Groom,
                        RoleInCertificate.Witness,
                        RoleInCertificate.Witness
                    };
                    break;
                case CertificateType.Other:
                    roles = new List<RoleInCertificate>
                    {
                        RoleInCertificate.Registrar,
                        RoleInCertificate.Other
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(CType), CType, null);
            }

            // filter to stored people based on the fact that their role matches a required role
            foreach (var reservedPerson in _storedPeople)
            {
                // compair its role to the roles
                foreach (var role in roles)
                {
                    if (reservedPerson.Role == role)
                    {
                        People.Add(reservedPerson);
                        _storedPeople.Remove(reservedPerson);
                        roles.Remove(role);
                    }
                }
            }

            // add the remaining roles, that were not yet added, to the people list
            foreach(var role in roles)
            {
                var newPersonInfo = new CertificatePersonInfo(role);
                People.Add(newPersonInfo);
            }

            SetVisibleFields(People);
            UpdateAvailableRoles();
        }

        private void UpdateAvailableRoles()
        {
            // avoid overriding! otherwise the binding does not always work
            AvailableRoles.Clear();
            foreach (var role in Enum.GetValues(typeof(RoleInCertificate)).Cast<RoleInCertificate>())
            {
                if (!People.Any(p => OneTimeRoles.Contains(p.Role) && p.Role == role))
                {
                    if (CType == CertificateType.Marriage && role == RoleInCertificate.Subject)
                    {
                        continue;
                    }
                    AvailableRoles.Add(role);
                }
            }
        }

        [RelayCommand]
        private void AddNewPersonInfo()
        {
            if (SelectedRole == null) return;

            foreach (var storedPerson in _storedPeople)
            {
                if (storedPerson.Role == SelectedRole)
                {
                    People.Add(storedPerson);
                    _storedPeople.Remove(storedPerson);
                    if (OneTimeRoles.Contains(SelectedRole.Value))
                    {
                        AvailableRoles.Remove(SelectedRole.Value);
                    }
                    SelectedRole = null;
                    return;
                }
            }

            People.Add(new CertificatePersonInfo(SelectedRole.Value));
            SetVisibleFields(People);
            if (OneTimeRoles.Contains(SelectedRole.Value))
            {
                AvailableRoles.Remove(SelectedRole.Value);
            }
            SelectedRole = null;
        }

        [RelayCommand]
        private void RemovePersonInfo(CertificatePersonInfo personInfo)
        {
            if (personInfo.HasValue())
                _storedPeople.Add(personInfo);
            People.Remove(personInfo);
            if (OneTimeRoles.Contains(personInfo.Role))
            {
                AvailableRoles.Add(personInfo.Role);
            }
            return;
        }

        private void SetVisibleFields(ObservableCollection<CertificatePersonInfo> peopleInfo)
        {
            foreach (var personInfo in peopleInfo)
            {
                List<RoleInCertificate> detailedBirth = new List<RoleInCertificate> { RoleInCertificate.Father, RoleInCertificate.Mother, RoleInCertificate.Spouse, RoleInCertificate.Mother_of_the_Bride, RoleInCertificate.Father_of_the_Bride, RoleInCertificate.Mother_of_the_Groom, RoleInCertificate.Father_of_the_Groom };
                List<RoleInCertificate> canNotBeDeath = new List<RoleInCertificate> { RoleInCertificate.Bride, RoleInCertificate.Groom };

                if (personInfo.Role == RoleInCertificate.Registrar)
                {
                    personInfo.SetVisibilityFields(VisibleAttributes.NameOnly, NameState.basic);
                    continue;
                } 
                else if(personInfo.Role == RoleInCertificate.Witness)
                {
                    personInfo.SetVisibilityFields(VisibleAttributes.detailedBirth, false);
                    continue;
                }
                else if (detailedBirth.Contains(personInfo.Role))
                {
                    personInfo.SetVisibilityFields(VisibleAttributes.detailedBirth, NameState.complete);
                    continue;
                }
                else if (canNotBeDeath.Contains(personInfo.Role))
                {
                    personInfo.SetVisibilityFields(VisibleAttributes.detailedBirth, false, NameState.complete);
                    continue;
                }
                else if (personInfo.Role == RoleInCertificate.Subject)
                {
                    if (CType == CertificateType.Birth)
                    {
                        personInfo.SetVisibilityFields(VisibleAttributes.basicBirth, NameState.complete);
                        continue;
                    }
                }

                personInfo.SetVisibilityFields(VisibleAttributes.full, NameState.complete);
            }
        }

    }
}
