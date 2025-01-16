using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models.Results
{
    /// <summary>
    /// Result model for `GET v1/wizard/registry-credentials`.
    /// </summary>
    public class GetRegistryCredentialsResult
    {
        [JsonProperty2("registry_url")]
        public string RegistryUrl { get; set; }

        [JsonProperty2("project")]
        public string Project { get; set; }

        [JsonProperty2("username")]
        public string Username { get; set; }

        [JsonProperty2("token")]
        public string Token { get; set; }
    }
}
