namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class JA_82SH : JablotronSensorDevice
    {
        internal JA_82SH(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
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