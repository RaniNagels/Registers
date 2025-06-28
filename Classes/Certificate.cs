using Newtonsoft.Json;
using Registers.Classes.Repositories;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Registers.Classes
{
    public class Certificate : IdItem
    {
        [JsonProperty("type")]
        public CertificateType Type { get; set; }

        [JsonProperty("date")]
        public DateTime Date {  get; set; }

        [JsonProperty("location")]
        public Guid Location { get; set; }

        [JsonProperty("certificate_persons")]
        public List<CertificatePersonInfo> RawPersonsData { get; set; }

        [JsonProperty("url")]
        public string? URL { get; set; }

        [JsonProperty("certificate_number")]
        public int CertificateNumber { get; set; }

        public override string ToString()
        {
            return $"[{Type}] nr.:{CertificateNumber}   ({Date.Day}/{Date.Month}/{Date.Year})";
        }

        public Location? GetLocation()
        {
            return DataRepository.Instance.Get<Location>(Location);
        }

        public override bool HasReferences()
        {
            // if the certificate exists, it is the reference
            return true;
        }

    }

    public enum CertificateType
    {
        Birth,
        Death,
        Marriage,
        Other
    }
}
