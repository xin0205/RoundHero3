using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace IO.Swagger.Model {

  /// <summary>
  /// 
  /// </summary>
  [DataContract]
  public class MatchmakerReleaseConfigResponse : BaseModel {
    /// <summary>
    /// Matchmaker configuration name. Must be unique.
    /// </summary>
    /// <value>Matchmaker configuration name. Must be unique.</value>
    [DataMember(Name="name", EmitDefaultValue=false)]
    [JsonProperty2(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Matchmaker configuration, parsed as a string.
    /// </summary>
    /// <value>Matchmaker configuration, parsed as a string.</value>
    [DataMember(Name="configuration", EmitDefaultValue=false)]
    [JsonProperty2(PropertyName = "configuration")]
    public string Configuration { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      StringBuilder sb = new StringBuilder();
      sb.Append("class MatchmakerReleaseConfigResponse {\n");
      sb.Append("  Name: ").Append(Name).Append("\n");
      sb.Append("  Configuration: ").Append(Configuration).Append("\n");
      sb.Append("}\n");
      return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public  new string ToJson() {
      return JsonConvert2.SerializeObject(this, Formatting.Indented);
    }

}
}
