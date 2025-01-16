using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models
{
    /// <summary>
    /// Shared model for `GetDeploymentStatusResult`, `StopActiveDeploymentResult`.
    /// </summary>
    public class SessionData
    {
        [JsonProperty2("session_id")]
        public string SessionId { get; set; }
            
        [JsonProperty2("status")]
        public string Status { get; set; }
            
        [JsonProperty2("ready")]
        public bool Ready { get; set; }
            
        [JsonProperty2("linked")]
        public bool Linked { get; set; }
            
        [JsonProperty2("kind")]
        public string Kind { get; set; }
            
        [JsonProperty2("user_count")]
        public string UserCount { get; set; }
    }
}
