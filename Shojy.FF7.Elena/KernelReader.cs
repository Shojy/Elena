using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using Shojy.FF7.Elena.Sections;

namespace Shojy.FF7.Elena
{
    public class KernelReader
    {
        public WeaponData WeaponData { get; protected set; }

        private Dictionary<KernelSection, byte[]> _kernelData;
        private KernelType _kernelFile;

        public KernelReader(string filePath, KernelType kernelFile = KernelType.KernelBin)
        {
            if (kernelFile == KernelType.Kernel2Bin)
            {
                throw new NotSupportedException("kernel2.bin is not yet supported.");
            }

            this._kernelFile = kernelFile;
            // Load file and decompress
            this._kernelData = Decompress(filePath);

            // Sections
            this.LoadSections();
        }

        private void LoadSections()
        {
            this.WeaponData = new WeaponData(this._kernelData[KernelSection.WeaponData]);
        }


        private static Dictionary<KernelSection, byte[]> Decompress(string path)
        {
            var kernelFile = new FileInfo(path);

            using (var fileStream = kernelFile.OpenRead())
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                var sections = new Dictionary<KernelSection, byte[]>();
                var offset = 0;

                for (var sectionIndex = 0; sectionIndex < 27; ++sectionIndex)
                {
                    var sectionCompressedLength = BitConverter.ToUInt16(bytes, offset + 0);
                    var sectionDecompressedLength = BitConverter.ToUInt16(bytes, offset + 2);
                    var fileType = BitConverter.ToUInt16(bytes, offset + 4);

                    var compressedSection = new byte[sectionCompressedLength];
                    Array.Copy(
                        bytes,
                        offset + 6,
                        compressedSection,
                        0,
                        sectionCompressedLength);

                    var decompressedSection = DecompressSection(compressedSection);
                    Console.WriteLine($"Section #{sectionIndex}: ComLen: {sectionCompressedLength} DecLen: {sectionDecompressedLength} Actual: {decompressedSection.Length}");
                    sections.Add((KernelSection)sectionIndex + 1, decompressedSection);

                    offset += 6 + sectionCompressedLength;
                }
                return sections;
            }

        }

        private static byte[] DecompressSection(byte[] compressedSection)
        {
            using(var compressedStream = new MemoryStream(compressedSection))
            using (var gzip = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var decompressedStream = new MemoryStream())
            {
                gzip.CopyTo(decompressedStream);
                return decompressedStream.ToArray();
            }
        }
    }
}
