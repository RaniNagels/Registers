using MvvmCross.Plugin.Messenger;
using Registers.Classes.ProjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Registers.Classes
{
    public enum RepositoryOptions
    {
        add,
        remove,
        update
    }

    public class MiddlePanelMessage : MvxMessage
    {
        public MiddlePanelMessage(object sender, System.Windows.Controls.UserControl middlePanel)
            : base(sender)
        {
            MiddlePanel = middlePanel;
        }

        public System.Windows.Controls.UserControl MiddlePanel { get; private set; }
    }

    public static class ServiceLocator
    {
        public static IMvxMessenger Messenger { get; } = new MvxMessengerHub();
    }

    public class CertificateMessage : MvxMessage
    {
        public CertificateMessage(object sender, RepositoryOptions option, Certificate certificate)
            : base(sender)
        {
            Certificate = certificate;
            Option = option;
        }

        public Certificate Certificate { get; private set; }
        public RepositoryOptions Option { get; private set; }
    }

    public class LocationMessage : MvxMessage
    {
        public LocationMessage(object sender, RepositoryOptions option, Location location)
            : base(sender)
        {
            Location = location;
            Option = option;
        }

        public Location Location { get; private set; }
        public RepositoryOptions Option { get; private set; }
    }

    public class ProjectFilePathMessage : MvxMessage
    {
        public ProjectFilePathMessage(object sender, string filePath)
            : base(sender)
        {
            ProjectFilePath = filePath;
        }

        public string ProjectFilePath { get; private set; }
    }
}
