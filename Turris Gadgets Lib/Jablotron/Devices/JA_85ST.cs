using System;

namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class JA_85ST : DefaultJablotronSensorDevice
    {
        private const string ButtonPatern = "BUTTON";
        private const string DefectPatern = "DEFECT";

        internal JA_85ST(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
            if (message.Contains(ButtonPatern))
            {
                OnButtonNotification(new ButtonEventArgs());
            }
            if (message.Contains(DefectPatern))
            {
                OnDefectNotification(new DefectEventArgs(message.Contains(TurrisDongle.ActActivePatern)));
            }
        }

        protected internal override void OnDispose()
        {
        }

        public event ButtonEventHandler ButtonNotification;

        protected virtual void OnButtonNotification(ButtonEventArgs e)
        {
            ButtonNotification?.Invoke(this, e);
        }

        public event DefectEventHandler DefectNotification;

        protected virtual void OnDefectNotification(DefectEventArgs e)
        {
            DefectNotification?.Invoke(this, e);
        }

        public override bool IsSensorCircuitPresent => false;
    }

    public delegate void ButtonEventHandler(object sender, ButtonEventArgs e);

    public class ButtonEventArgs : EventArgs
    {
        public ButtonEventArgs()
        {
        }
    }

    public delegate void DefectEventHandler(object sender, DefectEventArgs e);

    public class DefectEventArgs : EventArgs
    {
        public bool IsCircuitClosed { get; }

        public DefectEventArgs(bool isCircuitClosed)
        {
            IsCircuitClosed = isCircuitClosed;
        }
    }
}