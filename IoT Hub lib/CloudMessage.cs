using System;
using System.Collections.Generic;

namespace Pospa.NET.IoTHub
{
    public class CloudMessage
    {
        internal CloudMessage()
        {
            Properties = new Dictionary<string, string>();
            ExpiryTime = null;
        }

        public string Etag { get; set; }
        public long SequenceNumber { get; set; }
        public DateTime EnqueuedTime { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public string Ack { get; set; }
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public string UserId { get; set; }
        public string To { get; set; }
        internal string DataType { get; set; }
        internal string DataEncoding { get; set; }
        public IDictionary<string, string> Properties { get;}
    }
}