using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shojy.FF7.Elena.Characters
{
    [Flags]
    public enum LearnedLimits : ushort
    {
        LimitLv1_1 = 0x0001,
        LimitLv1_2 = 0x0002,
        LimitLv2_1 = 0x0008,
        LimitLv2_2 = 0x0010,
        LimitLv3_1 = 0x0040,
        LimitLv3_2 = 0x0080,
        LimitLv4 = 0x2000
    }
}
