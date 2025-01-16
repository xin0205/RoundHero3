using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models.Results
{
    public class StopActiveDeploymentResult
    {
        [JsonProperty2("message")]
        public string Message { get; set; }
        
        [JsonProperty2("deployment_summary")]
        public DeploymentSummaryData DeploymentSummary { get; set; }
        

        public class DeploymentSummaryData
        {
            [JsonProperty2("request_id")]
            public string RequestId { get; set; }
            
            [JsonProperty2("fqdn")]
            public string Fqdn { get; set; }
            
            [JsonProperty2("app_name")]
            public string AppName { get; set; }
            
            [JsonProperty2("app_version")]
            public string AppVersion { get; set; }
            
            [JsonProperty2("current_status")]
            public string CurrentStatus { get; set; }
            
            [JsonProperty2("running")]
            public bool Running { get; set; }
            
            [JsonProperty2("whitelisting_active")]
            public bool WhitelistingActive { get; set; }
            
            [JsonProperty2("start_time")]
            public string StartTime { get; set; }
            
            [JsonProperty2("removal_time")]
            public string RemovalTime { get; set; }
            
            [JsonProperty2("elapsed_time")]
            public int ElapsedTime { get; set; }
            
            [JsonProperty2("last_status")]
            public string LastStatus { get; set; }
            
            [JsonProperty2("error")]
            public bool Error { get; set; }
            
            [JsonProperty2("error_detail")]
            public string ErrorDetail { get; set; }
            
            [JsonProperty2("ports")]
            public PortsData Ports { get; set; }
            
            [JsonProperty2("public_ip")]
            public string PublicIp { get; set; }
            
            [JsonProperty2("sessions")]
            public SessionData[] Sessions { get; set; }
            
            [JsonProperty2("location")]
            public LocationData Location { get; set; }
            
            [JsonProperty2("tags")]
            public string[] Tags { get; set; }
            
            [JsonProperty2("sockets")]
            public string Sockets { get; set; }
            
            [JsonProperty2("sockets_usage")]
            public string SocketsUsage { get; set; }
            
            [JsonProperty2("command")]
            public string Command { get; set; }
            
            [JsonProperty2("arguments")]
            public string Arguments { get; set; }
            
        }
        
        public class PortsData
        {

        }
    }
}
