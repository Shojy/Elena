using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shojy.FF7.Elena.Attacks
{
    public enum StatusChangeType
    {
        None, Inflict, Cure, Swap
    }

    public class StatusChange
    {
        public StatusChangeType Type { get; set; }
        public int Amount { get; set; }

        public StatusChange(byte value)
        {
            if (value == 0xFF)
            {
                Type = StatusChangeType.None;
                Amount = 0;
            }
            else if ((value & 0x40) != 0)
            {
                Type = StatusChangeType.Cure;
                Amount = value - 0x40;
            }
            else if ((value & 0x80) != 0)
            {
                Type = StatusChangeType.Swap;
                Amount = value - 0x80;
            }
            else
            {
                Type = StatusChangeType.Inflict;
                Amount = value;
            }
        }
    }
}
