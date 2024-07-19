using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shojy.FF7.Elena.Battle
{
    public class SpellIndex
    {
        public SpellType SpellType { get; set; }
        public byte SpellID { get; set; }
        public byte SectionIndex { get; set; }

        public SpellIndex(byte spellID, byte data)
        {
            SpellID = spellID;
            if (data == 0xFF)
            {
                SpellType = SpellType.Unlisted;
            }
            else
            {
                var holder = new byte[] { data };
                var source = new BitArray(holder);
                bool[] indexBits = new bool[8], typeBits = new bool[8];

                int i;
                for (i = 0; i < 5; ++i)
                {
                    indexBits[i] = source[i];
                }
                for (i = 0; i < 3; ++i)
                {
                    typeBits[i] = source[i + 5];
                }

                //get index
                var converter = new BitArray(indexBits);
                converter.CopyTo(holder, 0);
                SectionIndex = holder[0];

                //get type
                converter = new BitArray(typeBits);
                converter.CopyTo(holder, 0);
                SpellType = (SpellType)holder[0];
            }
        }
    }
}
