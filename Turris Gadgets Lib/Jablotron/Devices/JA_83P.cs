using System;

namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class JA_83P : JablotronSensorDevice
    {
        public JA_83P(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
        }

        protected internal override void OnDispose()
        {
        }
    }
}