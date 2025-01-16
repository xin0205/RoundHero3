using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models.Results
{
    /// <summary>
    /// Result model for `POST v1/deploy`.
    /// </summary>
    public class CreateDeploymentResult
    {
        [JsonProperty2("request_id")]
        public string RequestId { get; set; }
        
        [JsonProperty2("request_dns")]
        public string RequestDns { get; set; }
        
        [JsonProperty2("request_app")]
        public string RequestApp { get; set; }
        
        [JsonProperty2("request_version")]
        public string RequestVersion { get; set; }
        
        [JsonProperty2("request_user_count")]
        public int RequestUserCount { get; set; }
        
        [JsonProperty2("city")]
        public string City { get; set; }
        
        [JsonProperty2("country")]
        public string Country { get; set; }
        
        [JsonProperty2("continent")]
        public string Continent { get; set; }
        
        [JsonProperty2("administrative_division")]
        public string AdministrativeDivision { get; set; }
        
        [JsonProperty2("tags")]
        public string[] Tags { get; set; }
        
        [JsonProperty2("container_log_storage")]
        public ContainerLogStorageData ContainerLogStorage { get; set; }
        

        public class ContainerLogStorageData
        {
            [JsonProperty2("enabled")]
            public bool Enabled { get; set; }
            
            [JsonProperty2("endpoint_storage")]
            public string EndpointStorage { get; set; }
        }
    }
}
