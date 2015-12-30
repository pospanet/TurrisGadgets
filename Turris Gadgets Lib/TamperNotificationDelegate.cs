using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pospa.NET.TurrisGadgets
{
    public delegate void TamperNotificationEventHandler(object sender, TamperNotificationEventArgs e);

    public class TamperNotificationEventArgs : EventArgs
    {
        public TamperNotificationEventArgs()
        {
        }
    }
}
