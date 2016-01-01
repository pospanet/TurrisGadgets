using System;

namespace Pospa.NET.TurrisGadgets.Jablotron
{
    public delegate void LowBatteryNotificationEventHandler(object sender, LowBatteryNotificationEventArgs e);

    public class LowBatteryNotificationEventArgs : EventArgs
    {
        public LowBatteryNotificationEventArgs()
        {
        }
    }
}