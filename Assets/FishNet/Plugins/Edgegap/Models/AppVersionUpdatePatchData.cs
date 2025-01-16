using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edgegap
{
    //[Obsolete("Use UpdateAppVersionRequest")] // MIRROR CHANGE: commented this out to avoid import warnings
    public struct AppVersionUpdatePatchData
    {

        [JsonProperty2("docker_repository")]
        public string DockerRegistry;

        [JsonProperty2("docker_image")]
        public string DockerImage;

        [JsonProperty2("docker_tag")]
        public string DockerTag;

    }
}
