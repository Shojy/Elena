using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shojy.FF7.Elena.Inventory
{
    public class InventoryItem
    {
        public ushort Item;
        public int Amount;

        public void ParseData(byte[] data)
        {
            var source = new BitArray(data);
            var indexBits = new bool[16];
            var amountBits = new bool[8];

            int i;
            for (i = 0; i < 9; ++i)
            {
                indexBits[i] = source[i];
            }
            for (i = 0; i < 7; ++i)
            {
                amountBits[i] = source[i + 9];
            }

            var converter = new BitArray(indexBits);
            var indexBytes = new byte[2];
            converter.CopyTo(indexBytes, 0);
            Item = BitConverter.ToUInt16(indexBytes, 0);

            converter = new BitArray(amountBits);
            var amountBytes = new byte[1];
            converter.CopyTo(amountBytes, 0);
            Amount = amountBytes[0];
        }
    }
}
