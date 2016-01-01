using System;

namespace Pospa.NET.TurrisGadgets.Jablotron
{
    public delegate void TamperNotificationEventHandler(object sender, TamperNotificationEventArgs e);

    public class TamperNotificationEventArgs : EventArgs
    {
        public bool IsCircuitClosed { get; }
        public TamperNotificationEventArgs(bool isCircuitClosed)
        {
            IsCircuitClosed = isCircuitClosed;
        }
    }
}
