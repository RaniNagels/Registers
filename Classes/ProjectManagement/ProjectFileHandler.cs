using Registers.Classes.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes.ProjectManagement
{
    public static class ProjectFileHandler
    {
        private const string _fileExtension = ".stamboom";

        public static List<ProjectInfo> RecentProjects { get; private set; } = new List<ProjectInfo>();
        
        public static ProjectInfo CreateNewProject(string filePath)
        {
            return new ProjectInfo
            {
                FilePath = filePath,
                ProjectName = Path.GetFileNameWithoutExtension(filePath),
                ProjectDescription = "This is a stamboom project.",
                Created = DateTime.Now,
                LastModified = DateTime.Now,
                Author = Environment.UserName
            };
        }


        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Project file not found.", filePath);

            DataRepository.Instance.Load(filePath);
        }

        public static void Save()
        {
            var projectInfo = DataRepository.Instance.GetProjectInfo();
            projectInfo.LastModified = DateTime.Now;
            DataRepository.Instance.Save();
        }

        public static string GetFileSize(string filePath)
        {
            var projectInfo = DataRepository.Instance.GetProjectInfo();
            var fileInfo = new FileInfo(projectInfo.FilePath);
            return FormatSize(fileInfo.Length);
        }

        private static string FormatSize(long bytes)
        {
            if (bytes >= 1024 * 1024)
                return $"{bytes / (1024 * 1024.0):0.##} MB";
            if (bytes >= 1024)
                return $"{bytes / 1024.0:0.##} KB";
            return $"{bytes} B";
        }
    }
}
