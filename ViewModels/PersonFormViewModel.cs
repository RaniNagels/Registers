using CommunityToolkit.Mvvm.ComponentModel;
using Registers.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.ViewModels
{
    public enum PersonFormType
    {
        Subject,
        Mother,
        Father,
        Daughter,
        Son,
        Witness,
        registrar
    }

    public partial class PersonFormViewModel : ObservableObject
    {
        public PersonFormType Role { get; }

        [ObservableProperty] private string? _firstName;
        [ObservableProperty] private string? _middleNames;
        [ObservableProperty] private string? _lastName;

        [ObservableProperty] private TimedOccupation? _occupation;
        [ObservableProperty] private TimedLocation? _currentResidence;
        [ObservableProperty] private int _age;

        [ObservableProperty] private TimedLocation? _placeOfBirth;
        [ObservableProperty] private TimedLocation? _placeOfDeath;

        [ObservableProperty] private DateTime? _dateOfBirth;
        [ObservableProperty] private DateTime? _dateOfDeath;
    }
}
