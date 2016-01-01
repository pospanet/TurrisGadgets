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

    public class AC_80L : JablotronDevice
    {
        internal AC_80L(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class AC_81M : JablotronDevice
    {
        internal AC_81M(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class AC_82SH : JablotronDevice
    {
        internal AC_82SH(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class AC_83M : JablotronDevice
    {
        internal AC_83M(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class AC_83P : JablotronDevice
    {
        internal AC_83P(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class AC_85ST : JablotronDevice
    {
        internal AC_85ST(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class AC_86K : JablotronDevice
    {
        internal AC_86K(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        public int Address2nd => (_type + 0x10)*65536 + _address;
        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
    public class AC_86K_2nd : JablotronDevice
    {
        internal AC_86K_2nd(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }
    }

    public class AC_82N : JablotronDevice
    {
        internal AC_82N(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
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