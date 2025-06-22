using Newtonsoft.Json;

namespace Registers.Classes
{
    class Person
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("first_name")]
        public string? FirstName { get; set; }

        [JsonProperty("middle_names")]
        public string? MiddleNames { get; set; }

        [JsonProperty("last_name")]
        public string? LastName { get; set; }

        [JsonProperty("gender")]
        public Gender Gender { get; set; }


        [JsonProperty("birth_date")]
        public DateTime BirhtDate { get; set; }

        [JsonProperty("birth_location")]
        public TimedLocation? BirthLocation { get; set; }


        [JsonProperty("is_alive")]
        public bool IsAlive { get; set; }


        [JsonProperty("death_date")]
        public DateTime DeathDate { get; set; }

        [JsonProperty("death_location")]
        public TimedLocation? DeathLocation { get; set; }


        [JsonProperty("certifcate_ids")]
        public List<Guid> CertificateIds { get; set; } = new();

        [JsonProperty("Relationships")]
        public List<Relationship> Relationships { get; set; } = new();

        [JsonProperty("Occupations")]
        public List<TimedOccupation> Occupations { get; set; } = new();

        [JsonProperty("Residences")]
        public List<TimedLocation?> Residences { get; set; } = new();

        public void SetBirthData(DateTime? birthDate, TimedLocation? location)
        {

        }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
