using System;
using System.Collections;
using System.Linq;
using Pospa.NET.TurrisGadgets.Jablotron;

namespace Pospa.NET.TGManager
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
