using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes.Repositories
{
    public class JsonRepository<T> where T : class
    {
        private readonly string _filePath;

        private T _item;
        public T Item { get; }

        public JsonRepository(string filePath)
        {
            _filePath = filePath;
            _item = Activator.CreateInstance<T>();
            Item = _item;
        }

        public void Load()
        {
            if (!File.Exists(_filePath)) return;

            var json = File.ReadAllText(_filePath);
            var list = JsonConvert.DeserializeObject<T>(json);

            _item = list;
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(Item, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

       //public void Add(T item)
       //{
       //    _item.Add(item);
       //}

        //public void Remove(T item)
        //{
        //    _item.Remove(item);
        //}

        public T Get()
        {
            return _item;
        }

        //public void Clear()
        //{
        //    _items.Clear();
        //}

        //public void Update(T original_item, T updated_item)
        //{
        //    int index = _item.IndexOf(original_item);
        //    _item[index] = updated_item;
        //}
    }
}
