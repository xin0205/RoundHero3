using Newtonsoft.Json;
using System.Collections.Generic;

namespace Edgegap
{
    public struct DeployPostData
    {
        [JsonProperty2("app_name")]
        public string AppName { get; set; }

        [JsonProperty2("version_name")]
        public string AppVersionName { get; set; }

        [JsonProperty2("ip_list")]
        public IList<string> IpList { get; set; }

        public DeployPostData(string appName, string appVersionName, IList<string> ipList)
        {
            AppName = appName;
            AppVersionName = appVersionName;
            IpList = ipList;
        }
    }
}
