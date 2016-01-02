namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class JA_82SH : DefaultJablotronSensorDevice
    {
        internal JA_82SH(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        public override bool IsSensorCircuitPresent => false;

        protected internal override void ProcessMessage(string message)
        {
        }

        protected internal override void OnDispose()
        {
        }
    }
}