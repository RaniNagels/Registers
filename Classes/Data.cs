using Newtonsoft.Json;
using Registers.Classes.ProjectManagement;
using Registers.Classes.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes
{
    public class Data
    {
        [JsonProperty("ProjectInfo")]
        public ProjectInfo ProjectInfo { get; set; }

        [JsonProperty("certifactes")]
        public List<Certificate> Certificates { get; set; }

        [JsonProperty("people")]
        public List<Person> People { get; set; }

        [JsonProperty("locations")]
        public List<Location> Locations { get; set; }

        [JsonProperty("countries")]
        public List<Country> Countries { get; set; }

        public Data()
        {
            ProjectInfo = new ProjectInfo();
            Certificates = new List<Certificate>();
            People = new List<Person>();
            Locations = new List<Location>();
            Countries = new List<Country>();
        }

        public void AddItem<T>(T item) where T : IdItem
        {
            if (item is Certificate cert)       Certificates.Add(cert);
            else if (item is Person person)     People.Add(person);
            else if (item is Location loc)      Locations.Add(loc);
            else if (item is Country country)   Countries.Add(country);
        }

        public void RemoveItem<T>(T item) where T : IdItem
        {
            if (item is Certificate cert)       Remove(Certificates, cert);
            else if (item is Person person)     Remove(People, person);
            else if (item is Location loc)      Remove(Locations, loc);
            else if (item is Country country)   Remove(Countries, country);
        }

        private void Remove<T>(List<T> items, T item) where T : IdItem
        {
            var match = items.FirstOrDefault(x => x.Id == item.Id);
            if (match != null)
                items.Remove(match);
        }

        public void UpdateItem<T>(T item) where T : IdItem
        {
            if (item is Certificate cert)       Update(Certificates, cert);
            else if (item is Person person)     Update(People, person);
            else if(item is Location loc)       Update(Locations, loc);
            else if (item is Country country)   Update(Countries, country);
        }

        private void Update<T>(List<T> items, T updatedItem) where T : IdItem
        {
            var index = items.FindIndex(x => x.Id == updatedItem.Id);
            if (index != -1)
            {
                items[index] = updatedItem;
            }
        }
    }

    public abstract class IdItem
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        // must be overriden in subclasses
        public abstract bool HasReferences();
    }
}
