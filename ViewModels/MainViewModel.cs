using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Registers.Classes;
using Registers.Classes.Repositories;
using System.Windows.Controls;
using Registers.UserControls;
using Registers.UserControls.Country;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Collections.Specialized;
using MvvmCross.Plugin.Messenger;
using System.DirectoryServices;
using Registers.Classes.Helpers;
using Registers.Classes.ProjectManagement;
using CommunityToolkit.Mvvm.Messaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.IO;

namespace Registers.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // panel details with viewmodels (left and right panels)
        [ObservableProperty]
        private System.Windows.Controls.UserControl _certificateDetail = new System.Windows.Controls.UserControl();

        private CertificateViewModel _certifVM;

        [ObservableProperty]
        private System.Windows.Controls.UserControl _locationDetail = new System.Windows.Controls.UserControl();
        [ObservableProperty] private Visibility _locationPanelVisibility = Visibility.Visible;
        private LocationViewModel _locationVM;

        [ObservableProperty]
        private System.Windows.Controls.UserControl _countryDetail = new System.Windows.Controls.UserControl();
        [ObservableProperty] private Visibility _countryPanelVisibility = Visibility.Visible;
        private CountryViewModel _countryVM;

        [ObservableProperty] private Visibility _rightPanelInUse = Visibility.Visible;
        [ObservableProperty] private int _selectedTabIndex = 0;

        [ObservableProperty]
        private System.Windows.Controls.UserControl _startUpPage = new System.Windows.Controls.UserControl();
        private StartupViewModel _startUpVM;

        private JsonRepository<ConfigurationSettings> _configSettings;

        //-------------------------------------------------------------
        // Set the middle panel - using the messenger
        private MvxSubscriptionToken _panelToken;
        private IMvxMessenger _panelMessenger;

        [ObservableProperty]
        private System.Windows.Controls.UserControl _selectedDetail = new System.Windows.Controls.UserControl();

        private void OnMiddlePanelMessageReceived(MiddlePanelMessage message)
        {
            var userControl = message.MiddlePanel;
            SelectedDetail = userControl;
            _certifVM.UpdateFilteredItems();
            _locationVM.UpdateFilteredItems();
            _countryVM.UpdateFilteredItems();
        }

        //-------------------------------------------------------------
        // project information
        private MvxSubscriptionToken _projectToken;
        private IMvxMessenger _projectMessenger;

        private string _projectFilePath;

        private void OnProjectFileMessageReceived(ProjectFilePathMessage message)
        {
            _projectFilePath = message.ProjectFilePath;
            StartUpPage.Visibility = Visibility.Collapsed;
            LoadInAllData();
        }

        [ObservableProperty] private string _windowTitle = "Registers - Stamboom Application";

        //-------------------------------------------------------------
        private void LoadInAllData()
        {
            _panelMessenger = ServiceLocator.Messenger;
            _panelToken = _panelMessenger.Subscribe<MiddlePanelMessage>(OnMiddlePanelMessageReceived);

            GetRepository().Load(_projectFilePath);
            UpdateWindowTitle();

            _certifVM = new CertificateViewModel(_panelMessenger);
            CertificateDetail = new CertificatePanel { DataContext = _certifVM };

            _locationVM = new LocationViewModel(_panelMessenger);
            LocationDetail = new LocationPanel { DataContext = _locationVM };

            _countryVM = new CountryViewModel(_panelMessenger);
            CountryDetail = new CountryPanel { DataContext = _countryVM };
        }

        [RelayCommand]
        public void StartApplication()
        {
            _projectMessenger = ServiceLocator.Messenger;
            _projectToken = _projectMessenger.Subscribe<ProjectFilePathMessage>(OnProjectFileMessageReceived);

            // get path to enbededd resources
            var resourcePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            var configPath = System.IO.Path.Combine(resourcePath, "config.json");

            _configSettings = new JsonRepository<ConfigurationSettings>(configPath);
            _configSettings.Load();

            _startUpVM = new StartupViewModel(_projectMessenger, _configSettings.Get().RecentFiles);
            StartUpPage = new StartUpPage { DataContext = _startUpVM };
        }

        [RelayCommand]
        public void Save()
        {
            if (StartUpPage.Visibility == Visibility.Collapsed)
            {
                var result = System.Windows.MessageBox.Show(
                    "Do you wish to Save?",
                    "Saving File",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    ProjectFileHandler.Save();
                }

            }
            _configSettings.Item.RecentFiles = _startUpVM.RecentFiles;
            _configSettings.Save();
        }

        private DataRepository GetRepository()
        {
            return DataRepository.Instance;
        }

        [RelayCommand]
        private void SwitchCountryPanelVisibility()
        {
            CountryPanelVisibility = CountryPanelVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            UpdateRightPanelVisibility();
            if (CountryPanelVisibility == Visibility.Visible)
            {
                SelectedTabIndex = 1;
            }
        }

        [RelayCommand]
        private void SwitchLocationPanelVisibility()
        {
            LocationPanelVisibility = LocationPanelVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            UpdateRightPanelVisibility();
            if (LocationPanelVisibility == Visibility.Visible)
            {
                SelectedTabIndex = 0;
            }
        }

        private bool AreAllPanelsInvisible()
        {
            return LocationPanelVisibility == Visibility.Collapsed &&
                   CountryPanelVisibility == Visibility.Collapsed;
        }

        private void UpdateRightPanelVisibility()
        {
            RightPanelInUse = AreAllPanelsInvisible() ? Visibility.Collapsed : Visibility.Visible;
        }

        [RelayCommand]
        private void OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Stamboom Project (*.stamboom)|*.stamboom",
                Title = "Open Stamboom Project"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                UpdateStartUpViewModel(dialog.FileName);
                Save();
                LoadInAllData();
            }
        }

        [RelayCommand]
        private void CreateNewFile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Stamboom Project (*.stamboom)|*.stamboom",
                Title = "Create New Stamboom Project",
                DefaultExt = "stamboom"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                UpdateStartUpViewModel(dialog.FileName);
                Save();
                var projectInfo = ProjectFileHandler.CreateNewProject(_projectFilePath);
                DataRepository.Instance.Save(projectInfo);
                LoadInAllData();
            }

        }

        private void UpdateStartUpViewModel(string projectFilePath)
        {
            _projectFilePath = projectFilePath;
            _startUpVM.CurrentFile = _projectFilePath;
            _startUpVM.AddRecentFile(_projectFilePath);
        }

        [RelayCommand]
        private void Close(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        [RelayCommand]
        private void SaveAs()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Stamboom Project (*.stamboom)|*.stamboom",
                Title = "Save as..",
                DefaultExt = "stamboom"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                UpdateStartUpViewModel(dialog.FileName);
                UpdateWindowTitle();
                DataRepository.Instance.SaveAs(dialog.FileName);
            }
        }

        private void UpdateWindowTitle()
        {
            if (!string.IsNullOrEmpty(_projectFilePath))
            {
                WindowTitle = "Registers - " + Path.GetFileNameWithoutExtension(_projectFilePath);
            }
            else
            {
                WindowTitle = "Registers - Stamboom Application";
            }
        }
    }
}
