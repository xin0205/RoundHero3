using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models
{
    /// <summary>Used in `GetDeploymentStatus`.</summary>
    public class DeploymentPortsData
    {
        [JsonProperty2("external")]
        public int External { get; set; }
        
        [JsonProperty2("internal")]
        public int Internal { get; set; }
        
        [JsonProperty2("protocol")]
        public string Protocol { get; set; }
        
        [JsonProperty2("name")]
        public string PortName { get; set; }
        
        [JsonProperty2("tls_upgrade")]
        public bool TlsUpgrade { get; set; }
        
        [JsonProperty2("link")]
        public string Link { get; set; }
        
        [JsonProperty2("proxy")]
        public int? Proxy { get; set; }
    }
}
