using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pospa.NET.TurrisGadgets.Jablotron;

namespace TG_Manager
{
    public static class Extensions
    {
        public static BitArray GetSignature(this JablotronDevice device)
        {
            byte index = device.GetIndex();
            BitArray bitArray = new BitArray(BitConverter.GetBytes(index).ToArray());
            bitArray.Length = 5;
            return bitArray;
        }
    }
}
