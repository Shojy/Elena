using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Xml;

namespace Shojy.FF7.Elena.Extensions
{
    public static class TextExtensions
    {
        public static string ToFFString(this byte[] bytes)
        {
            var length = bytes.Length;
            var text = new List<byte>();

            for (var chr = 0; chr < length; ++chr)
            {
                if (bytes[chr] < 0x60)
                {
                    text.Add((byte)(bytes[chr] + 0x20));
                }
                else if (bytes[chr] == 0xD0)
                {
                    text.Add(0x20);
                }
                else if (bytes[chr] < 0xE1)
                {
                    // Non-English characters are within this range. These should be added later,
                    // but for now I'm just treating them as a space.
                    text.Add(0x3F);
                }
                else if (bytes[chr] == 0xE2)
                {
                    text.Add(0x9);
                }
                else if (bytes[chr] == 0xE3)
                {
                    text.Add(0x2);
                }
                else if (bytes[chr] == 0xEA)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Cloud}"));
                }
                else if (bytes[chr] == 0xEB)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Barret}"));
                }
                else if (bytes[chr] == 0xEC)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Tifa}"));
                }
                else if (bytes[chr] == 0xED)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Aeris}"));
                }
                else if (bytes[chr] == 0xEE)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Red XIII}"));
                }
                else if (bytes[chr] == 0xEF)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Yuffie}"));
                }
                else if (bytes[chr] == 0xF0)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Caith Sith}"));
                }
                else if (bytes[chr] == 0xF1)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Vincent}"));
                }
                else if (bytes[chr] == 0xF2)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Cid}"));
                }
                else if (bytes[chr] == 0xF3)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Party 1}"));
                }
                else if (bytes[chr] == 0xF4)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Party 2}"));
                }
                else if (bytes[chr] == 0xF5)
                {
                    text.AddRange(Encoding.ASCII.GetBytes("{Party 3}"));
                }
                else if (bytes[chr] < 0xFE || bytes[chr] == 0xF8) 
                {
                    // This is a function with a parameter following. We can skip both.
                    ++chr;
                }
                else if (bytes[chr] == 0xFF)
                {
                    // That's all folks!
                    break;
                }
                else
                {
                    text.Add(0x3F);
                }
            }

            return Encoding.ASCII.GetString(text.ToArray());
        }
    }
}