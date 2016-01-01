using System;

namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class RC_86K : JablotronDevice
    {
        private const string PanicEventPatern = "PANIC";
        private const string KeypressEventPatern = "ARM:";
        private const string KeypressArmEventPatern = "ARM:1";
        internal RC_86K(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        public int Address2nd => (_type + 0x10)*65536 + _address;
        protected internal override void ProcessMessage(string message)
        {
            if (message.Contains(PanicEventPatern))
            {
                OnPanicNotification(new PanicEventArgs());
            }
            if (message.Contains(KeypressEventPatern))
            {
                OnKeypressNotification(new KeypressEventArgs(message.Contains(KeypressArmEventPatern)));
            }
        }
        public event KeypressEventHandler KeypressNotification;

        protected virtual void OnKeypressNotification(KeypressEventArgs e)
        {
            KeypressNotification?.Invoke(this, e);
        }

        public event PanicEventHandler PanicNotification;

        protected virtual void OnPanicNotification(PanicEventArgs e)
        {
            PanicNotification?.Invoke(this, e);
        }
    }
    public class RC_86K_2nd : RC_86K
    {
        internal RC_86K_2nd(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }
    }
    public delegate void KeypressEventHandler(object sender, KeypressEventArgs e);

    public class KeypressEventArgs : EventArgs
    {
        public bool IsArmed { get; }

        public KeypressEventArgs(bool arm)
        {
            IsArmed = arm;
        }
    }
    public delegate void PanicEventHandler(object sender, PanicEventArgs e);

    public class PanicEventArgs : EventArgs
    {
        public PanicEventArgs()
        {
        }
    }
}