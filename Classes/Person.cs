using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Registers.Classes.Repositories;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace Registers.Classes
{
    public class Person : IdItem
    {
        [JsonProperty("first_name")]
        public string? FirstName { get; set; }
        public List<string>? FirstNameVariations { get; set; } = new();

        [JsonProperty("middle_names")]
        public List<string>? MiddleNames { get; set; }

        [JsonProperty("last_name")]
        public string? LastName { get; set; }
        public List<string>? LastNameVariations { get; set; }

        [JsonProperty("nickname")]
        public string? Nickname { get; set; }

        //=========================================================================

        [JsonProperty("birth_date")]
        public DateTime? BirthDate { get; set; }

        [JsonProperty("birth_location")]
        public TimedLocation? BirthLocation { get; set; }

        //=========================================================================

        [JsonProperty("is_alive")]
        public bool IsAlive { get; set; }

        [JsonProperty("death_date")]
        public DateTime? DeathDate { get; set; }

        [JsonProperty("death_location")]
        public TimedLocation? DeathLocation { get; set; }

        //=========================================================================

        [JsonProperty("spouses")]
        public List<MarriageInfo> Marriages { get; set; } = new();

        [JsonProperty("Relationships")]
        public List<Relationship> Relationships { get; set; } = new();

        //=========================================================================

        [JsonProperty("Occupations")]
        public List<TimedOccupation> Occupations { get; set; } = new();

        [JsonProperty("Residences")]
        public List<TimedLocation?> Residences { get; set; } = new();

        //=========================================================================

        [JsonProperty("certifcate_ids")]
        public List<Guid> CertificateIds { get; set; } = new();

        //=========================================================================
        //=========================================================================

        public override string ToString()
        {
            string dates = "";
            if (BirthDate.HasValue)
                dates += BirthDate.Value.ToString("yyyy");
            else dates += "?";

            if (IsAlive)
                dates += " - present";
            else
            {
                dates += " - ";
                if (DeathDate.HasValue)
                    dates += DeathDate.Value.ToString("yyyy");
                else dates += "?";
            }
            return $"{FirstName} {LastName} ({dates})";
        }

        public override bool HasReferences()
        {
            foreach (var certId in CertificateIds)
            {
                var cert = DataRepository.Instance.Get<Certificate>(certId);
                if (cert != null)
                    return true;
            }
            return false;
        }
    }

    public partial class CertificatePersonInfo : ObservableObject
    {
        public Guid? PersonId { get; set; } // is the person already in the database?
        public RoleInCertificate Role { get; set; }

        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }
        public string? BirthLocation { get; set; }

        public DateTime? DeathDate { get; set; }
        public string? DeathLocation { get; set; }

        public string? Occupation { get; set; }
        public string? Residence { get; set; }

        [JsonIgnore]
        public List<Visibility> VisibleFields { get; set; } = 
            new List<Visibility> { 
                Visibility.Visible, 
                Visibility.Visible,
                Visibility.Visible,
                Visibility.Visible,
                Visibility.Visible,
                Visibility.Visible,
                Visibility.Visible,
                Visibility.Visible,
                Visibility.Visible
            };

        [JsonIgnore]
        VisibleAttributes visibilityState;

        private bool _isDeceased = false;
        public bool IsDeceased
        {
            get => _isDeceased;
            set
            {
                _isDeceased = value;
                OnPropertyChanged();
                OnCheckBoxChanged();
            }
        }

        [JsonIgnore]
        public Visibility CheckBoxVisibility { get; set; }

        public CertificatePersonInfo(RoleInCertificate role)
        {
            Role = role;
        }

        public void SetVisibilityFields(VisibleAttributes va, NameState ns = NameState.complete)
        {
            visibilityState = va;
            switch(va)
            {
                case VisibleAttributes.NameOnly:
                    VisibleFields.Clear();

                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(GetMiddleNameVisibility(ns));
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(BirthDate == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(BirthLocation == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(DeathDate == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(DeathLocation == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(Occupation == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(Residence == null ? Visibility.Collapsed : Visibility.Visible);

                    break;
                case VisibleAttributes.basicBirth:
                    VisibleFields.Clear();

                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(GetMiddleNameVisibility(ns));
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(DeathDate == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(DeathLocation == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(Occupation == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(Residence == null ? Visibility.Collapsed : Visibility.Visible);

                    break;
                case VisibleAttributes.detailedBirth:
                    VisibleFields.Clear();

                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(GetMiddleNameVisibility(ns));
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(DeathDate == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(DeathLocation == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);

                    break;
                case VisibleAttributes.basicDeath:
                    VisibleFields.Clear();

                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(GetMiddleNameVisibility(ns));
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(BirthDate == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(BirthLocation == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Occupation == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(Residence == null ? Visibility.Collapsed : Visibility.Visible);

                    break;
                case VisibleAttributes.detailedDeath:
                    VisibleFields.Clear();

                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(GetMiddleNameVisibility(ns));
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(BirthDate == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(BirthLocation == null ? Visibility.Collapsed : Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);

                    break;
                case VisibleAttributes.full:
                    VisibleFields.Clear();

                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);
                    VisibleFields.Add(Visibility.Visible);

                    break;
            }

            if (va == VisibleAttributes.detailedBirth)
            {
                CheckBoxVisibility = Visibility.Visible;
            }
            else
            {
                CheckBoxVisibility = Visibility.Collapsed;
            }
        }

        public void SetVisibilityFields(VisibleAttributes va, bool canBeDeath, NameState ns = NameState.basic)
        {
            SetVisibilityFields(va, ns);
            if (canBeDeath)
            {
                CheckBoxVisibility = Visibility.Visible;
            }
            else
            {
                CheckBoxVisibility = Visibility.Collapsed;
            }
        }

        private Visibility GetMiddleNameVisibility(NameState ns)
        {
            switch (ns)
            {
                case NameState.basic:
                    return Visibility.Collapsed;
                case NameState.complete:
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public bool HasValue()
        {
            return !string.IsNullOrEmpty(FirstName) ||
                   !string.IsNullOrEmpty(MiddleNames) ||
                   !string.IsNullOrEmpty(LastName) ||
                   BirthDate.HasValue ||
                   !string.IsNullOrEmpty(BirthLocation) ||
                   DeathDate.HasValue ||
                   !string.IsNullOrEmpty(DeathLocation) ||
                   !string.IsNullOrEmpty(Residence) ||
                   !string.IsNullOrEmpty(Occupation);
        }

        private void OnCheckBoxChanged()
        {
            if (IsDeceased)
            {
                VisibleFields[3] = Visibility.Collapsed;
                VisibleFields[4] = Visibility.Collapsed;
                VisibleFields[5] = Visibility.Visible;
                VisibleFields[6] = Visibility.Visible;
                OnPropertyChanged(nameof(VisibleFields));
            }
            else
            {
                VisibleFields[3] = Visibility.Visible;
                VisibleFields[4] = Visibility.Visible;
                VisibleFields[5] = Visibility.Collapsed;
                VisibleFields[6] = Visibility.Collapsed;
                OnPropertyChanged(nameof(VisibleFields));
            }
        }

        [RelayCommand]
        private void ShowAllAttributes()
        {
            SetVisibilityFields(VisibleAttributes.full);
            OnPropertyChanged(nameof(VisibleFields));
        }
    }

    public enum RoleInCertificate
    {
        Subject,
        Father,
        Mother,
        Spouse,
        Daughter,
        Son,
        Grandchild,
        Sibling,
        Grandparent,
        Witness,
        Mother_of_the_Groom,
        Father_of_the_Groom,
        Mother_of_the_Bride,
        Father_of_the_Bride,
        Bride,
        Groom,
        Registrar,
        Other
    }

    public enum VisibleAttributes
    {
        NameOnly,       // firstname, (middlenames), lastname
        basicBirth,     // firstname, (middlenames), lastname, birthdate, birthplace
        detailedBirth,  // firstname, (middlenames), lastname, birthdate, birthplace, occupation, residence
        basicDeath,     // firstname, (middlenames), lastname, deathdate, deathplace
        detailedDeath,  // firstname, (middlenames), lastname, deathdate, deathplace, occupation, residence
        full            // firstname, (middlenames), lastname, birthdate, birthplace, deathdate, deathplace, occupation, residence
    }
    public enum NameState
    {
        basic,
        complete
    }
}