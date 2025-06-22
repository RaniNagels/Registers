using Newtonsoft.Json;
using Registers.Classes.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Registers.Classes
{
    public class Location : IdItem
    {
        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("country")]
        public Guid? CountryId { get; set; }

        public override string ToString()
        {
            if (CountryId == null || CountryId == Guid.Empty)
                return $"{City}";

            var country = DataRepository.Instance.Get<Country>()
                .FirstOrDefault(c => c.Id == CountryId);
            return $"{City}, {country}";
        }

        public Location(string? city, Guid? countryId)
        {
            this.Id = Guid.NewGuid();
            this.City = city;
            this.CountryId = countryId;
        }

        public Location()
        { }

        public Country? GetCountry()
        {
            if (CountryId == null || CountryId == Guid.Empty)
                return null;
            else
            {
                Guid countryId = CountryId.Value;
                return DataRepository.Instance.Get<Country>(countryId);
            }
        }

        public override bool HasReferences()
        {
            return DataRepository.Instance.Get<Certificate>()
                .Any(c => c.Location == this.Id);
        }
    }

    public class Country : IdItem
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        public Country(string? name)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
        }

        public override string ToString()
        {
            return Name ?? "Unknown Country";
        }

        public Country() { }

        public override bool HasReferences()
        {
            return DataRepository.Instance.Get<Location>()
                .Any(l => l.CountryId == this.Id);
        }
    }

    public class TimedLocation
    {
        public DateTime? Date { get; set; } // when was this person in this location
        public Guid? Location { get; set; }

        public TimedLocation(DateTime? date, Guid? locationId)
        {
            this.Date = date;
            this.Location = locationId;
        }
    }

    public class RawGeoData
    {
        [JsonProperty("countries")]
        public List<Country> Countries { get; set; } = new();

        [JsonProperty("locations")]
        public List<Location> Locations { get; set; } = new();
    }
}
