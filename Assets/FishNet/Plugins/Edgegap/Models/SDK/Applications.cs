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
  public class Applications {
    /// <summary>
    /// Gets or Sets _Applications
    /// </summary>
    [DataMember(Name="applications", EmitDefaultValue=false)]
    [JsonProperty2(PropertyName = "applications")]
    public List<Application> _Applications { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      StringBuilder sb = new StringBuilder();
      sb.Append("class Applications {\n");
      sb.Append("  _Applications: ").Append(_Applications).Append("\n");
      sb.Append("}\n");
      return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson() {
      return JsonConvert2.SerializeObject(this, Formatting.Indented);
    }

}
}
