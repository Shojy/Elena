using Shojy.FF7.Elena.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Shojy.FF7.Elena
{
    public class KernelReader
    {
        #region Private Fields

        private readonly Dictionary<KernelSection, byte[]> _kernelData;
        private KernelType _kernelFile;

        #endregion Private Fields

        #region Public Constructors

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

        #endregion Public Constructors

        #region Public Properties

        public AccessoryData AccessoryData { get; protected set; }
        public TextSection AccessoryDescriptions { get; protected set; }
        public TextSection AccessoryNames { get; protected set; }
        public ArmorData ArmorData { get; protected set; }
        public TextSection ArmorDescriptions { get; protected set; }
        public TextSection ArmorNames { get; protected set; }
        public TextSection BattleText { get; protected set; }
        public TextSection CommandDescriptions { get; protected set; }
        public TextSection CommandNames { get; protected set; }
        public ItemData ItemData { get; set; }
        public TextSection ItemDescriptions { get; protected set; }
        public TextSection ItemNames { get; protected set; }
        public KeyItemData KeyItemData { get; protected set; }
        public TextSection KeyItemDescriptions { get; protected set; }
        public TextSection KeyItemNames { get; protected set; }
        public TextSection MagicDescriptions { get; protected set; }
        public TextSection MagicNames { get; protected set; }
        public MateriaData MateriaData { get; protected set; }
        public TextSection MateriaDescriptions { get; protected set; }
        public TextSection MateriaNames { get; protected set; }
        public TextSection SummonAttackNames { get; protected set; }
        public WeaponData WeaponData { get; protected set; }
        public TextSection WeaponDescriptions { get; protected set; }
        public TextSection WeaponNames { get; protected set; }

        #endregion Public Properties

        #region Private Methods

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
            using (var compressedStream = new MemoryStream(compressedSection))
            using (var gzip = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var decompressedStream = new MemoryStream())
            {
                gzip.CopyTo(decompressedStream);
                return decompressedStream.ToArray();
            }
        }

        private void LoadSections()
        {
            this.CommandDescriptions = new TextSection(this._kernelData[KernelSection.CommandDescriptions]);
            this.MagicDescriptions = new TextSection(this._kernelData[KernelSection.MagicDescriptions]);
            this.ItemDescriptions = new TextSection(this._kernelData[KernelSection.ItemDescriptions]);
            this.WeaponDescriptions = new TextSection(this._kernelData[KernelSection.WeaponDescriptions]);
            this.ArmorDescriptions = new TextSection(this._kernelData[KernelSection.ArmorDescriptions]);
            this.AccessoryDescriptions = new TextSection(this._kernelData[KernelSection.AccessoryDescriptions]);
            this.MateriaDescriptions = new TextSection(this._kernelData[KernelSection.MateriaDescriptions]);
            this.KeyItemDescriptions = new TextSection(this._kernelData[KernelSection.KeyItemDescriptions]);
            this.CommandNames = new TextSection(this._kernelData[KernelSection.CommandNames]);
            this.MagicNames = new TextSection(this._kernelData[KernelSection.MagicNames]);
            this.ItemNames = new TextSection(this._kernelData[KernelSection.ItemNames]);
            this.WeaponNames = new TextSection(this._kernelData[KernelSection.WeaponNames]);
            this.ArmorNames = new TextSection(this._kernelData[KernelSection.ArmorNames]);
            this.AccessoryNames = new TextSection(this._kernelData[KernelSection.AccessoryNames]);
            this.MateriaNames = new TextSection(this._kernelData[KernelSection.MateriaNames]);
            this.KeyItemNames = new TextSection(this._kernelData[KernelSection.KeyItemNames]);
            this.BattleText = new TextSection(this._kernelData[KernelSection.BattleText]);
            this.SummonAttackNames = new TextSection(this._kernelData[KernelSection.SummonAttackNames]);

            this.WeaponData = new WeaponData(
                this._kernelData[KernelSection.WeaponData],
                this.WeaponNames.Strings,
                this.WeaponDescriptions.Strings);

            this.ItemData = new ItemData(
                this._kernelData[KernelSection.ItemData],
                this.ItemNames.Strings,
                this.ItemDescriptions.Strings);

            this.ArmorData = new ArmorData(
                this._kernelData[KernelSection.ArmorData],
                this.ArmorNames.Strings,
                this.ArmorDescriptions.Strings);

            this.AccessoryData = new AccessoryData(
                this._kernelData[KernelSection.AccessoryData],
                this.AccessoryNames.Strings,
                this.AccessoryDescriptions.Strings);

            this.MateriaData = new MateriaData(
                this._kernelData[KernelSection.MateriaData],
                this.MateriaNames.Strings,
                this.MateriaDescriptions.Strings);

            this.KeyItemData = new KeyItemData(this.KeyItemNames.Strings, this.KeyItemDescriptions.Strings);
        }

        #endregion Private Methods
    }
}