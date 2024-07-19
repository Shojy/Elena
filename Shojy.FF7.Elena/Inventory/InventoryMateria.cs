using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shojy.FF7.Elena.Inventory
{
    public class InventoryMateria
    {
        public byte Index { get; set; }
        public int CurrentAP { get; set; }

        public void ParseData(byte[] data)
        {
            Index = data[0];

            var ap = new byte[4];
            Array.Copy(data, 1, ap, 0, 3);
            CurrentAP = BitConverter.ToInt32(ap);
        }
    }
}
