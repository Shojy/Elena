using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shojy.FF7.Elena.Attacks
{
    public enum ConditionSubmenu : byte
    {
        PartyHP = 0x00,
        PartyMP = 0x01,
        PartyStatus = 0x02,
        None = 0xFF
    }
}
