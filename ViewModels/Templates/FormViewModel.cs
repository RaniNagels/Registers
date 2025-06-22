using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;
using Registers.Classes;

namespace Registers.ViewModels.Templates
{
    public abstract partial class FormViewModel<TDataType> : ObservableObject, IFormViewModel<TDataType> where TDataType : IdItem
    {
        // member variables
        protected Guid _id;

        [ObservableProperty] private string? _buttonName;
        [ObservableProperty] private string? _panelName;
        [ObservableProperty] private Visibility? _visibility;
        [ObservableProperty] private bool _isEnabled;


        // Events
        public event Action<TDataType>? Saved;
        public event Action<Guid>? Removed;
        public event Action? Canceled;

        // =================================================================
        // abstract functions that need to be overriden
        protected abstract Guid GetIdFromModel(TDataType model);
        protected abstract void LoadModelData(TDataType model);
        protected abstract TDataType CreateModelFromState(Guid id);

        // =================================================================
        // Constructors
        protected FormViewModel()
        {
            ButtonName = "Create";
            PanelName = "Add " + typeof(TDataType).Name + ":";
            Visibility = System.Windows.Visibility.Hidden;
        }

        protected FormViewModel(TDataType model)
        {
            _id = GetIdFromModel(model);
            LoadModelData(model);
            ButtonName = "Save";
            PanelName = "Edit " + typeof(TDataType).Name + ":";
            if (model is not Certificate)
            {
                if (model.HasReferences())
                {
                    IsEnabled = false;
                }
                else
                {
                    IsEnabled = true;
                }
            }
            Visibility = System.Windows.Visibility.Visible;
        }

        // =================================================================
        [RelayCommand]
        private void Save()
        {
            if (_id == Guid.Empty)
                _id = Guid.NewGuid();

            var model = CreateModelFromState(_id);
            Saved?.Invoke(model);
        }

        [RelayCommand]
        private void Cancel()
        {
            Canceled?.Invoke();
        }

        [RelayCommand]
        private void Remove()
        {
            if (_id != Guid.Empty)
            {
                var result = System.Windows.MessageBox.Show(
                    "Are you sure you want to delete this " + typeof(TDataType).Name + "?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Removed?.Invoke(_id);
                }
            }
        }
    }

}
