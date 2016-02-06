using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pospa.NET.TurrisGadgets.Jablotron;

namespace Pospa.NET.TurrisGadgets
{
    public class DataMessage
    {
        public DataMessage()
        {
        }

        public DataMessage(string message)
        {
            RawMessage = message;
        }
        [JsonProperty(PropertyName = "RawMessage")]
        public string RawMessage { get; set; }

        [JsonProperty(PropertyName = "RawDeviceAddress")]
        public string RawDeviceAddress { get; set; }

        [JsonProperty(PropertyName = "DeviceAddress")]
        public int DeviceAddress { get; set; }
        [JsonProperty(PropertyName = "DeviceType")]
        public JablotronDevicType DeviceType { get; set; }
        [JsonProperty(PropertyName = "LowBattery")]
        public bool LowBattery { get; set; }
        [JsonProperty(PropertyName = "Tamper")]
        public bool Tamper { get; set; }
    }
}
