using System;

namespace Pospa.NET.TurrisGadgets.Jablotron
{
    public delegate void RelayStateChangeEventHandler(object sender, RelayStateChangeEventArgs e);

    public class RelayStateChangeEventArgs : EventArgs
    {
        public bool IsOn { get; }

        public RelayStateChangeEventArgs(bool isOn)
        {
            IsOn = isOn;
        }
    }
}