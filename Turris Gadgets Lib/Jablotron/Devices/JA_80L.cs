using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class JA_80L : JablotronDevice
    {
        internal JA_80L(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
        }

        protected internal override void ProcessMessage(string message)
        {
        }

        protected internal override void OnDispose()
        {
        }

        public Task Alarm()
        {
            throw new NotImplementedException();
        }
    }
}
