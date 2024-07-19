using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shojy.FF7.Elena.Battle
{
    public class StatCurve
    {
        public byte[] Gradients { get; } = new byte[8];
        public sbyte[] Bases { get; } = new sbyte[8];

        public void ParseData(byte[] data)
        {
            for (int i = 0; i < 8; i++)
            {
                Gradients[i] = data[i * 2];
                Bases[i] = (sbyte)data[(i * 2) + 1];
            }
        }
    }
}
