using System.Linq;
using System.Threading.Tasks;

namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class AC_88 : JablotronDevice
    {
        private const string RelayIsOnMessagePatern = "RELAY:1";
        private bool _isRelayOn;
        internal AC_88(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
            _isRelayOn = false;
        }

        private bool IsFirstListed()
        {
            return
                _turrisDongle.GetRegisteredDevices()
                    .First(d => d.GetDeviceType().Equals(JablotronDevicType.AC_88))
                    .Address.Equals(_address);
        }

        public event RelayStateChangeEventHandler RelayStateChangeNotification;

        protected internal override void ProcessMessage(string message)
        {
            _isRelayOn = message.Contains(RelayIsOnMessagePatern);
            OnRelayStateChangeNotification(new RelayStateChangeEventArgs(_isRelayOn));
        }

        protected internal override void OnDispose()
        {
        }

        protected virtual void OnRelayStateChangeNotification(RelayStateChangeEventArgs e)
        {
            RelayStateChangeNotification?.Invoke(this, e);
        }

        public void SetCurrentRealyState(bool isOn)
        {
            _isRelayOn = isOn;
        }

        public async Task ToggleRelayStateAsync()
        {
        }

        public async Task SetRelayAsync(bool on)
        {
            //this.SendMessageAsync()
        }
    }
}