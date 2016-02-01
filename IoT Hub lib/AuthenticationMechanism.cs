using Newtonsoft.Json;

namespace Pospa.NET.IoTHub
{
    public sealed class AuthenticationMechanism
    {
        public AuthenticationMechanism()
        {
            this.SymmetricKey = new SymmetricKey();
        }

        [JsonProperty(PropertyName = "symmetricKey")]
        public SymmetricKey SymmetricKey { get; set; }
    }
}
