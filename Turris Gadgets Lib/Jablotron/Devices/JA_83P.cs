using System;

namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class JA_83P : JablotronDevice
    {
        private const string SensorPatern = "SENSOR";
        private const string BeaconPatern = "BEACON";

        internal JA_83P(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            if (message.Contains(SensorPatern))
            {
                OnSensorNotification(new SensorEventArgs());
            }
            if (message.Contains(BeaconPatern))
            {
                OnBeaconNotification(new BeaconEventArgs());
            }
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
        public SensorEventArgs()
        {
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