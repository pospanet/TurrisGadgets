using System;
using System.Threading.Tasks;

namespace Pospa.NET.TurrisGadgets.Jablotron
{
    public abstract class JablotronSensorDevice : JablotronDevice
    {
        private const string SensorPatern = "SENSOR";
        private const string BeaconPatern = "BEACON";

        public bool IsSensorActivated { get; private set; }

        internal JablotronSensorDevice(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
            IsSensorActivated = false;
        }

        internal override async Task OnMessageReceiverAsync(string message)
        {
            if (message.Contains(SensorPatern))
            {
                OnSensorNotification(new SensorEventArgs(message.Contains(TurrisDongle.ActActivePatern)));
                IsSensorActivated = true;
            }
            if (message.Contains(BeaconPatern))
            {
                OnBeaconNotification(new BeaconEventArgs());
                IsSensorActivated = false;
            }
            await base.OnMessageReceiverAsync(message);
        }

        public event SensorEventHandler SensorNotification;

        protected virtual void OnSensorNotification(SensorEventArgs e)
        {
            SensorNotification?.Invoke(this, e);
        }

        public event BeaconEventHandler BeaconNotification;

        protected virtual void OnBeaconNotification(BeaconEventArgs e)
        {
            BeaconNotification?.Invoke(this, e);
        }
    }

    public delegate void SensorEventHandler(object sender, SensorEventArgs e);

    public class SensorEventArgs : EventArgs
    {
        public bool IsCircuitClosed { get; }
        public SensorEventArgs(bool isCircuitClosed)
        {
            IsCircuitClosed = isCircuitClosed;
        }
    }

    public delegate void BeaconEventHandler(object sender, BeaconEventArgs e);

    public class BeaconEventArgs : EventArgs
    {
        public BeaconEventArgs()
        {
        }
    }
}