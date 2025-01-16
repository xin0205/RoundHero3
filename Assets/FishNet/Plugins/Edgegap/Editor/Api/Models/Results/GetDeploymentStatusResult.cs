using System.Collections.Generic;
using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models.Results
{
    /// <summary>
    /// Result model for `GET v1/status/{request_id}`.
    /// API Doc | https://docs.edgegap.com/api/#tag/Deployments/operation/deployment-status-get
    /// </summary>
    public class GetDeploymentStatusResult
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
        public int? ElapsedTime { get; set; }
        
        [JsonProperty2("last_status")]
        public string LastStatus { get; set; }
        
        [JsonProperty2("error")]
        public bool Error { get; set; }
        
        [JsonProperty2("error_detail")]
        public string ErrorDetail { get; set; }
        
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
        
        /// <summary>
        /// TODO: Server should swap `ports` to an array of DeploymentPortsData (instead of an object of dynamic unknown objects).
        /// <example>
        /// {
        ///     "7777", {}
        /// },
        /// {
        ///     "Some Port Name", {}
        /// }
        /// </example>
        /// </summary>
        [JsonProperty2("ports")]
        public Dictionary<string, DeploymentPortsData> PortsDict { get; set; }
    }
}
