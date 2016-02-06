using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pospa.NET.IoTHub
{
    public class Device
    {
        // default constructor for deserialization
        internal Device()
        {
        }

        public Device(string id)
        {
            Id = id;
            ConnectionState = DeviceConnectionState.Disconnected;
            LastActivityTime = StatusUpdatedTime = ConnectionStateUpdatedTime = DateTime.MinValue;
        }

        [JsonProperty(PropertyName = "deviceId")]
        public string Id { get; internal set; }

        [JsonProperty(PropertyName = "generationId")]
        public string GenerationId { get; internal set; }

        [JsonProperty(PropertyName = "etag")]
        public string ETag { get; set; }

        [JsonProperty(PropertyName = "connectionState")]
        [JsonConverter(typeof (StringEnumConverter))]
        public DeviceConnectionState ConnectionState { get; internal set; }

        [JsonProperty(PropertyName = "status")]
        [JsonConverter(typeof (StringEnumConverter))]
        public DeviceStatus Status { get; set; }

        [JsonProperty(PropertyName = "statusReason")]
        public string StatusReason { get; set; }

        [JsonProperty(PropertyName = "connectionStateUpdatedTime")]
        public DateTime ConnectionStateUpdatedTime { get; internal set; }

        [JsonProperty(PropertyName = "statusUpdatedTime")]
        public DateTime StatusUpdatedTime { get; internal set; }

        [JsonProperty(PropertyName = "lastActivityTime")]
        public DateTime LastActivityTime { get; internal set; }

        [JsonProperty(PropertyName = "cloudToDeviceMessageCount")]
        public int CloudToDeviceMessageCount { get; internal set; }

        [JsonProperty(PropertyName = "authentication")]
        public AuthenticationMechanism Authentication { get; set; }
    }

    public class ServiceStatistics
    {
        public ServiceStatistics()
        {
        }

        [JsonProperty(PropertyName = "totalConnectedDevices")]
        public long ConnectedDevices { get; set; }
    }

    public class DeviceIdentitiesStatistics
    {
        public DeviceIdentitiesStatistics()
        {
        }
        [JsonProperty(PropertyName = "totalDeviceCount")]
        public long TotalDeviceCount { get; set; }
        [JsonProperty(PropertyName = "enabledDeviceCount")]
        public long EnabledDeviceCount { get; set; }
        [JsonProperty(PropertyName = "disabledDeviceCount")]
        public long DisabledDeviceCount { get; set; }
    }
}