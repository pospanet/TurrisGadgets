using System.Collections.Generic;

namespace Pospa.NET.IoTHub
{
    public class DeviceMessage
    {
        public DeviceMessage(object data)
        {
            Properties = new Dictionary<string, string>();
            MessageId = string.Empty;
            CorrelationId = string.Empty;
            UserId = string.Empty;
            Data = data;
        }

        public Dictionary<string, string> Properties { get; }
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public string UserId { get; set; }
        public object Data { get; set; }
    }
}