using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pospa.NET.IoTHub
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceStatus
    {
        [EnumMember(Value = "enabled")]
        Enabled = 0,

        [EnumMember(Value = "disabled")]
        Disabled,
    }
}
