using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models.Results
{
    /// <summary>
    /// Result model for:
    /// - `POST 1/app/{app_name}/version`
    /// - `PATCH v1/app/{app_name}/version/{version_name}`
    /// POST API Doc | https://docs.edgegap.com/api/#tag/Applications/operation/application-post
    /// PATCH API Doc | https://docs.edgegap.com/api/#tag/Applications/operation/app-versions-patch
    /// </summary>
    public class UpsertAppVersionResult
    {
        [JsonProperty2("success")]
        public bool Success { get; set; }

        [JsonProperty2("version")]
        public VersionData Version { get; set; }

        public class VersionData
        {
            [JsonProperty2("name")]
            public string VersionName { get; set; }

            [JsonProperty2("is_active")]
            public bool IsActive { get; set; }

            [JsonProperty2("docker_repository")]
            public string DockerRepository { get; set; }

            [JsonProperty2("docker_image")]
            public string DockerImage { get; set; }

            [JsonProperty2("docker_tag")]
            public string DockerTag { get; set; }

            [JsonProperty2("private_username")]
            public string PrivateUsername { get; set; }

            [JsonProperty2("private_token")]
            public string PrivateToken { get; set; }

            [JsonProperty2("req_cpu")]
            public int? ReqCpu { get; set; }

            [JsonProperty2("req_memory")]
            public int? ReqMemory { get; set; }

            [JsonProperty2("req_video")]
            public int? ReqVideo { get; set; }
            
            [JsonProperty2("max_duration")]
            public int? MaxDuration { get; set; }

            [JsonProperty2("use_telemetry")]
            public bool UseTelemetry { get; set; }

            [JsonProperty2("inject_context_env")]
            public bool InjectContextEnv { get; set; }

            [JsonProperty2("whitelisting_active")]
            public bool WhitelistingActive { get; set; }

            [JsonProperty2("force_cache")]
            public bool ForceCache { get; set; }

            [JsonProperty2("cache_min_hour")]
            public int? CacheMinHour { get; set; }

            [JsonProperty2("cache_max_hour")]
            public int? CacheMaxHour { get; set; }

            [JsonProperty2("time_to_deploy")]
            public int? TimeToDeploy { get; set; }

            [JsonProperty2("enable_all_locations")]
            public bool EnableAllLocations { get; set; }

            [JsonProperty2("session_config")]
            public SessionConfigData SessionConfig { get; set; }

            [JsonProperty2("ports")]
            public PortsData[] Ports { get; set; }

            [JsonProperty2("probe")]
            public ProbeData Probe { get; set; }

            [JsonProperty2("envs")]
            public EnvsData[] Envs { get; set; }

            [JsonProperty2("verify_image")]
            public bool VerifyImage { get; set; }

            [JsonProperty2("termination_grace_period_seconds")]
            public int? TerminationGracePeriodSeconds { get; set; }

            [JsonProperty2("endpoint_storage")]
            public string EndpointStorage { get; set; }

            [JsonProperty2("command")]
            public string Command { get; set; }

            [JsonProperty2("arguments")]
            public string Arguments { get; set; }
        }

        public class SessionConfigData
        {
            [JsonProperty2("kind")]
            public string Kind { get; set; }

            [JsonProperty2("sockets")]
            public int? Sockets { get; set; }

            [JsonProperty2("autodeploy")]
            public bool Autodeploy { get; set; }

            [JsonProperty2("empty_ttl")]
            public int? EmptyTtl { get; set; }

            [JsonProperty2("session_max_duration")]
            public int? SessionMaxDuration { get; set; }
        }

        public class PortsData
        {
            [JsonProperty2("port")]
            public int? Port { get; set; }

            [JsonProperty2("protocol")]
            public string Protocol { get; set; }

            [JsonProperty2("to_check")]
            public bool ToCheck { get; set; }

            [JsonProperty2("tls_upgrade")]
            public bool TlsUpgrade { get; set; }

            [JsonProperty2("name")]
            public string PortName { get; set; }
        }

        public class ProbeData
        {
            [JsonProperty2("optimal_ping")]
            public int? OptimalPing { get; set; }

            [JsonProperty2("rejected_ping")]
            public int? RejectedPing { get; set; }
        }

        public class EnvsData
        {
            [JsonProperty2("key")]
            public string Key { get; set; }

            [JsonProperty2("value")]
            public string Value { get; set; }

            [JsonProperty2("is_secret")]
            public bool IsSecret { get; set; }
        }

    }
}
