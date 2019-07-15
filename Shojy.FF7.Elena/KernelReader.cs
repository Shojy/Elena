using Shojy.FF7.Elena.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Shojy.FF7.Elena
{
    /// <summary>
    /// Kernel reader used for accessing data from FF7's Kernel files.
    /// </summary>
    public class KernelReader
    {
        #region Private Fields

        private readonly Dictionary<KernelSection, byte[]> _kernelData;
        private KernelType _kernelFile;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates a new instance of the Kernel file reader
        /// </summary>
        /// <param name="filePath">Path to the KERNEL.BIN or kernel2.bin file to open.</param>
        /// <param name="kernelFile">Whether the file is KERNEL.BIN or kernel2.bin</param>
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

        /// <summary>
        /// Kernel section 8, merged with Names (section 23) and descriptions (Section 15)
        /// </summary>
        public AccessoryData AccessoryData { get; protected set; }

        /// <summary>
        /// Kernel section 15
        /// </summary>
        public TextSection AccessoryDescriptions { get; protected set; }

        /// <summary>
        /// Kernel section 23
        /// </summary>
        public TextSection AccessoryNames { get; protected set; }

        /// <summary>
        /// Kernel section 7
        /// </summary>
        public ArmorData ArmorData { get; protected set; }

        /// <summary>
        /// Kernel section 14
        /// </summary>
        public TextSection ArmorDescriptions { get; protected set; }

        /// <summary>
        /// Kernel section 22
        /// </summary>
        public TextSection ArmorNames { get; protected set; }

        /// <summary>
        /// Kernel section 26
        /// </summary>
        public TextSection BattleText { get; protected set; }

        /// <summary>
        /// Kernel section 10
        /// </summary>
        public TextSection CommandDescriptions { get; protected set; }

        /// <summary>
        /// Kernel section 18
        /// </summary>
        public TextSection CommandNames { get; protected set; }

        /// <summary>
        /// Kernel section 5, merged with names (Section 20) and descriptions (section 12)
        /// </summary>
        public ItemData ItemData { get; set; }

        /// <summary>
        /// Kernel section 12
        /// </summary>
        public TextSection ItemDescriptions { get; protected set; }

        /// <summary>
        /// Kernel section 20
        /// </summary>
        public TextSection ItemNames { get; protected set; }

        /// <summary>
        /// Merged Section 25 (key Item Names) and 17 (Descriptions)
        /// </summary>
        public KeyItemData KeyItemData { get; protected set; }

        /// <summary>
        /// Kernel section 17
        /// </summary>
        public TextSection KeyItemDescriptions { get; protected set; }

        /// <summary>
        /// Kernel section 25
        /// </summary>
        public TextSection KeyItemNames { get; protected set; }

        /// <summary>
        /// Kernel section 19
        /// </summary>
        public TextSection MagicDescriptions { get; protected set; }

        /// <summary>
        /// Kernel section 24
        /// </summary>
        public TextSection MagicNames { get; protected set; }

        /// <summary>
        /// Kernel section 9, merged with names (section 24) and descriptions (section 16)
        /// </summary>
        public MateriaData MateriaData { get; protected set; }

        /// <summary>
        /// Kernel section 16
        /// </summary>
        public TextSection MateriaDescriptions { get; protected set; }

        /// <summary>
        /// Kernel section 24
        /// </summary>
        public TextSection MateriaNames { get; protected set; }

        /// <summary>
        /// Kernel section 27
        /// </summary>
        public TextSection SummonAttackNames { get; protected set; }

        /// <summary>
        /// Kernel section 6, merged with names (Section 21) and descriptions (section 13)
        /// </summary>
        public WeaponData WeaponData { get; protected set; }

        /// <summary>
        /// Kernel section 13
        /// </summary>
        public TextSection WeaponDescriptions { get; protected set; }

        /// <summary>
        /// Kernel section 21
        /// </summary>
        public TextSection WeaponNames { get; protected set; }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Reads a file path to a kernel file and decompresses each section
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Decompresses a byte[] using the GZIP format
        /// </summary>
        /// <param name="compressedSection">Compressed data</param>
        /// <returns>Decompressed data</returns>
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

        /// <summary>
        /// Parses and loads the data of each kernel section available.
        /// </summary>
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