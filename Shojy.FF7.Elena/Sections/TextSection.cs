using System;
using System.Collections.Generic;
using System.Diagnostics;
using Shojy.FF7.Elena.Extensions;

namespace Shojy.FF7.Elena.Sections
{
    public class TextSection
    {
        #region Private Fields

        private const byte Deliminator = 0xFF;
        private const byte LookupCommand = 0xF9;
        private readonly byte[] _sectionData;

        public List<string> Strings { get; }

        #endregion Private Fields

        #region Public Constructors

        public TextSection(byte[] sectionData, int offset = 0)
        {
            this._sectionData = sectionData;
            offset = BitConverter.ToUInt16(sectionData, 0);
            if (sectionData.Length < offset)
            {
                Console.WriteLine("Offset too big. Ignoring.");
                offset = 0;
            }
            var bytes = new byte[sectionData.Length - offset];
            Array.Copy(sectionData, offset, bytes, 0, bytes.Length);
            var decompressedData = DecompressText(bytes);
            this.Strings = ExtractStrings(decompressedData);
        }

        private static List<string> ExtractStrings(byte[] data)
        {
            var strings = new List<string>();

            var dataLength = data.Length;

            var currentString = new List<byte>();
            for (var i = 0; i < dataLength; ++i)
            {

                var character = data[i];

                if (character == Deliminator)
                {
                    var bytes = currentString.ToArray();
                    strings.Add(bytes.ToFFString());
                    currentString.Clear();
                }
                else
                {
                    currentString.Add(character);
                }
            }

            return strings;
        }

        private static byte[] DecompressText(byte[] data)
        {
            var text = new List<byte>();

            for (var index = 0; index < data.Length; ++index)
            {
                var character = data[index];
                // This is an encoding technique designed to make the raw data smaller. It is based
                // on the LZS compression method, but optimized for smaller files with fewer large
                // similar blocks. A byte following this value will tell the game's memory the location
                // of, and how much, text to read.
                // More info at: http://wiki.ffrtt.ru/index.php/FF7/FF_Text
                if (character == LookupCommand)
                {
                    var args = data[index + 1];
                    // The args byte is split into length, and offset. The first two bits are
                    // the length of data, and the remaining 6 are how far back to get it from.
                    // Length needs further calculation (L * 2 + 4) to provide the correct value.
                    var lookupLength = ((args & 0b11000000) >> 6) * 2 + 4;
                    var lookupOffset = (args & 0b00111111);

                    for (var i = 0; i < lookupLength; ++i)
                    {
                        var pos = index - lookupOffset + i;
                        try
                        {
                            text.Add(text[pos]);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.WriteLine($"Out of bounds lookup from index {index}");
                        }
                    }

                    // Skip processing the args byte
                    ++index;
                }
                else
                {
                    text.Add(character);
                }
            }

            return text.ToArray();
        }


        #endregion Public Constructors
    }
}