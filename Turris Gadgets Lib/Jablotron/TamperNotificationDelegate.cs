using System;

namespace Pospa.NET.TurrisGadgets.Jablotron
{
    public delegate void TamperNotificationEventHandler(object sender, TamperNotificationEventArgs e);

    public class TamperNotificationEventArgs : EventArgs
    {
        public TamperNotificationEventArgs()
        {
        }
    }
}
