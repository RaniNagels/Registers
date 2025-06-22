using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Registers.Classes.ProjectManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes.Repositories
{
    public partial class DataRepository : ObservableObject
    {
        private static readonly Lazy<DataRepository> _instance = new(() => new DataRepository());
        public static DataRepository Instance => _instance.Value;

        private DataRepository() {}

        private string _filePath = "D:\\02_Home\\Projects\\Stamboom\\App\\Registers\\Registers\\Resources\\Data.json";

        [ObservableProperty]
        private Data _registersData = new();

        public bool ChangesMade = false;

        public void Load()
        {
            if (!File.Exists(_filePath)) return;

            var json = File.ReadAllText(_filePath);
            var list = JsonConvert.DeserializeObject<Data>(json);

            RegistersData = new();
            if (list != null)
            {
                RegistersData = list;
            }
        }

        public void Load(string filePath)
        {
            if (!File.Exists(filePath)) return;
            _filePath = filePath;

            var json = File.ReadAllText(filePath);
            var list = JsonConvert.DeserializeObject<Data>(json);

            RegistersData = new();
            if (list != null)
            {
                RegistersData = list;
            }
        }

        public void Save()
        {
            JsonSerializerAndWrite();
        }

        public void Save(ProjectInfo projectInfo)
        {
            _filePath = projectInfo.FilePath;
            RegistersData = new();
            RegistersData.ProjectInfo = projectInfo;

            JsonSerializerAndWrite();
        }

        public void SaveAs(string filePath)
        {
            RegistersData.ProjectInfo.FilePath = filePath;
            RegistersData.ProjectInfo.ProjectName = Path.GetFileNameWithoutExtension(filePath);
            RegistersData.ProjectInfo.Created = DateTime.Now;
            RegistersData.ProjectInfo.LastModified = DateTime.Now;

            _filePath = filePath;

            JsonSerializerAndWrite();
        }

        private void JsonSerializerAndWrite()
        {
            var json = JsonConvert.SerializeObject(RegistersData, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void Add<T>(T item) where T : IdItem
        {
            ChangesMade = true;
            RegistersData.AddItem(item);
        }

        public void Remove<T>(T item) where T : IdItem
        {
            ChangesMade = true;
            RegistersData.RemoveItem(item);
        }

        public void Update<T>(T item) where T : IdItem
        {
            ChangesMade = true;
            RegistersData.UpdateItem(item);
        }

        public IEnumerable<T> Get<T>()
        {
            if (typeof(T) == typeof(Certificate))
            {
                return RegistersData.Certificates.Cast<T>();
            }
            else if (typeof(T) == typeof(Location))
            {
                return RegistersData.Locations.Cast<T>();
            }
            else if (typeof(T) == typeof(Country))
            {
                return RegistersData.Countries.Cast<T>();
            }
            else
            {
                return Enumerable.Empty<T>();
            }
        }

        public ProjectInfo GetProjectInfo()
        {
            return RegistersData.ProjectInfo;
        }

        public void SetProjectInfo(ProjectInfo projectInfo)
        {
            RegistersData.ProjectInfo = projectInfo;
            ChangesMade = true;
        }

        public T? Get<T>(Guid id) where T : class
        {
            if (typeof(T) == typeof(Certificate))
            {
                return RegistersData.Certificates.FirstOrDefault(c => c.Id == id) as T;
            }
            else if (typeof(T) == typeof(Location))
            {
                return RegistersData.Locations.FirstOrDefault(l => l.Id == id) as T;
            }
            else if (typeof(T) == typeof(Country))
            {
                return RegistersData.Countries.FirstOrDefault(c => c.Id == id) as T;
            }
            else
            {
                return default;
            }
        }
    }
}
