using System;

namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class JA_81M : DefaultJablotronSensorDevice
    {
        internal JA_81M(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
        }

        protected internal override void OnDispose()
        {
        }

        public override bool IsSensorCircuitPresent => true;
    }
}