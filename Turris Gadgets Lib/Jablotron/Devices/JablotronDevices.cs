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
    }

    public class JA_80L : JablotronDevice
    {
        internal JA_80L(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class JA_81M : JablotronDevice
    {
        internal JA_81M(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class JA_82SH : JablotronDevice
    {
        internal JA_82SH(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class JA_83M : JablotronDevice
    {
        internal JA_83M(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class JA_85ST : JablotronDevice
    {
        internal JA_85ST(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
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
    }
}