using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models
{
    public class LocationData
    {
        [JsonProperty2("city")]
        public string City { get; set; }
            
        [JsonProperty2("country")]
        public string Country { get; set; }
            
        [JsonProperty2("continent")]
        public string Continent { get; set; }
            
        [JsonProperty2("administrative_division")]
        public string AdministrativeDivision { get; set; }
            
        [JsonProperty2("timezone")]
        public string Timezone { get; set; }
            
        [JsonProperty2("latitude")]
        public double Latitude { get; set; }
            
        [JsonProperty2("longitude")]
        public double Longitude { get; set; }
    }
}
