using Newtonsoft.Json;

namespace Pospa.NET.IoTHub
{
    public sealed class SymmetricKey
    {
        [JsonProperty(PropertyName = "primaryKey")]
        public string PrimaryKey { get; set; }

        [JsonProperty(PropertyName = "secondaryKey")]
        public string SecondaryKey { get; set; }
    }
}
