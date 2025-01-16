using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models.Results
{
    /// <summary>
    /// Result model for `[GET | POST] v1/app`.
    /// POST API Doc | https://docs.edgegap.com/api/#tag/Applications/operation/application-post
    /// GET API Doc | https://docs.edgegap.com/api/#tag/Applications/operation/application-get
    /// </summary>
    public class GetCreateAppResult
    {
        [JsonProperty2("name")]
        public string AppName { get; set; }
        
        [JsonProperty2("is_active")]
        public bool IsActive { get; set; }
        
        /// <summary>Optional</summary>
        [JsonProperty2("is_telemetry_agent_active")]
        public bool IsTelemetryAgentActive { get; set; }
        
        [JsonProperty2("image")]
        public string Image { get; set; }
        
        [JsonProperty2("create_time")]
        public string CreateTimeStr { get; set; }
        
        [JsonProperty2("last_updated")]
        public string LastUpdatedStr { get; set; }
    }
}
