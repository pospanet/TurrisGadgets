using System;

namespace Pospa.NET.TurrisGadgets
{
    public delegate void LowBatteryNotificationEventHandler(object sender, LowBatteryNotificationEventArgs e);

    public class LowBatteryNotificationEventArgs : EventArgs
    {
        public LowBatteryNotificationEventArgs()
        {
        }
    }
}