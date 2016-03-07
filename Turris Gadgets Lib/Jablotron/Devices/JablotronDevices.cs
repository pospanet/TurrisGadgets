using System;

namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class AC_82 : JablotronDevice
    {
        internal AC_82(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        protected internal override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }

    public class UnknownJablotronDevice : JablotronDevice
    {
        internal UnknownJablotronDevice(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
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