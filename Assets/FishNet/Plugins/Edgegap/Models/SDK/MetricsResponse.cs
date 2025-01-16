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
  public class MetricsResponse {
    /// <summary>
    /// Gets or Sets Total
    /// </summary>
    [DataMember(Name="total", EmitDefaultValue=false)]
    [JsonProperty2(PropertyName = "total")]
    public TotalMetricsModel Total { get; set; }

    /// <summary>
    /// Gets or Sets Cpu
    /// </summary>
    [DataMember(Name="cpu", EmitDefaultValue=false)]
    [JsonProperty2(PropertyName = "cpu")]
    public MetricsModel Cpu { get; set; }

    /// <summary>
    /// Gets or Sets Mem
    /// </summary>
    [DataMember(Name="mem", EmitDefaultValue=false)]
    [JsonProperty2(PropertyName = "mem")]
    public MetricsModel Mem { get; set; }

    /// <summary>
    /// Gets or Sets Network
    /// </summary>
    [DataMember(Name="network", EmitDefaultValue=false)]
    [JsonProperty2(PropertyName = "network")]
    public NetworkMetricsModel Network { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      StringBuilder sb = new StringBuilder();
      sb.Append("class MetricsResponse {\n");
      sb.Append("  Total: ").Append(Total).Append("\n");
      sb.Append("  Cpu: ").Append(Cpu).Append("\n");
      sb.Append("  Mem: ").Append(Mem).Append("\n");
      sb.Append("  Network: ").Append(Network).Append("\n");
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
