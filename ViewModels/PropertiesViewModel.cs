using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmCross.Plugin.Messenger;
using Registers.Classes;
using Registers.Classes.ProjectManagement;
using Registers.Classes.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.ViewModels
{
    public partial class PropertiesViewModel : ObservableObject
    {
        [ObservableProperty] private ProjectInfo _projectInfo = new ProjectInfo();
        [ObservableProperty] private string _fileSize = string.Empty;

        private readonly IMvxMessenger _messenger;

        public PropertiesViewModel(IMvxMessenger messenger)
        {
            _messenger = messenger;
            ProjectInfo = DataRepository.Instance.RegistersData.ProjectInfo;
            FileSize = ProjectFileHandler.GetFileSize(ProjectInfo.FilePath);
            _messenger.Publish(new PropertiesVisibilityMessage(this, false));
        }

        [RelayCommand]
        private void CloseProperties()
        {
            DataRepository.Instance.SetProjectInfo(ProjectInfo);
            _messenger.Publish(new PropertiesVisibilityMessage(this, false));
        }
    }
}
