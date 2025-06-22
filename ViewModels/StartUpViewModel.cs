using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmCross.Plugin.Messenger;
using Registers.Classes;
using Registers.Classes.ProjectManagement;
using Registers.Classes.Repositories;
using Registers.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.ViewModels
{
    public partial class StartupViewModel : ObservableObject
    {
        [ObservableProperty] private List<string> _recentFiles = new List<string> {};
        [ObservableProperty] private List<string> _recentFileNames = new List<string> { };

        public string CurrentFile { get; set; }

        private readonly IMvxMessenger _messenger;

        public StartupViewModel(IMvxMessenger messenger, List<string> recentFiles)
        {
            _messenger = messenger;
            foreach (var file in recentFiles)
            {
                if (File.Exists(file))
                {
                    RecentFiles.Add(file);
                }
            }
            SetRecentFileNames();
        }

        private void SetRecentFileNames()
        {
            RecentFileNames.Clear();
            foreach (var fileName in RecentFiles)
            {
                RecentFileNames.Add(Path.GetFileName(fileName) ?? "Unknown");
            }
        }

        [RelayCommand]
        private void CreateNewFile()
        {
            // open file dialog to create a new file
            var dialog = new SaveFileDialog
            {
                Filter = "Stamboom Project (*.stamboom)|*.stamboom",
                Title = "Create New Stamboom Project",
                DefaultExt = "stamboom"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CurrentFile = dialog.FileName;
                AddRecentFile(CurrentFile);
                var projectInfo = ProjectFileHandler.CreateNewProject(CurrentFile);
                DataRepository.Instance.Save(projectInfo);
                _messenger.Publish(new ProjectFilePathMessage(this, projectInfo.FilePath));
            }
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
                CurrentFile = dialog.FileName;
                AddRecentFile(CurrentFile);
                _messenger.Publish(new ProjectFilePathMessage(this, CurrentFile));
            }
        }

        [RelayCommand]
        private void OpenSelectedFile(string fileName)
        {
            string filePath = RecentFiles.FirstOrDefault(f => Path.GetFileName(f) == fileName) ?? null;
            if (filePath == null) return;
            CurrentFile = filePath;
            AddRecentFile(filePath);
            _messenger.Publish(new ProjectFilePathMessage(this, CurrentFile));
        }

        public List<string> GetRecentFiles()
        {
            return RecentFiles;
        }

        public void AddRecentFile(string fileName)
        {
            if (!RecentFiles.Contains(fileName))
            {
                RecentFiles.Insert(0, fileName);
            }
            else
            {
                RecentFiles.Remove(fileName);
                RecentFiles.Insert(0, fileName);
            }

            // make sure that there are no more than 5 recent files
            if (RecentFiles.Count > 5)
            {
                RecentFiles.RemoveRange(5, RecentFiles.Count - 5);
            }

            SetRecentFileNames();
        }
    }
}
