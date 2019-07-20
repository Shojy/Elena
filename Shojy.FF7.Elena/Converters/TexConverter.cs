using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Shojy.FF7.Elena.Converters
{
    public class TexConverter
    {
        #region Public Methods

        public static Bitmap ToBitmap(Stream texData)
        {
            // Always reset the stream position just in case.
            texData.Position = 0;
            return ReadTexImage(texData);
            /*
            using (var binaryReader = new BinaryReader(texData))
            {
                var header = new TexFileHeader(
                    binaryReader.ReadBytes(
                        TexFileHeader.HeaderLength));

                var pixelFormat = new TexFilePixelFormat(
                    binaryReader.ReadBytes(
                        TexFilePixelFormat.FormatLength
                        ));

                
                var colorKeyPresent = binaryReader.ReadInt32();
                binaryReader.ReadInt32(); // runtime
                var referenceAlpha = binaryReader.ReadInt32();
                binaryReader.ReadBytes(0x28);



                var paletteData = ParsePaletteData(
                    binaryReader.ReadBytes(4 * header.PaletteSize));

                var bitmap = BitmapFromPaletteData(header, binaryReader, paletteData, referenceAlpha);

                return bitmap;
            }    */

        }

        private static Bitmap BitmapFromPaletteData(TexFileHeader header, BinaryReader binaryReader, IReadOnlyList<Color> paletteData, int referenceAlpha)
        {
            var numberOfPixels = header.Width * header.Height;
            var bitmap = new Bitmap(header.Width, header.Height);
            var pixelDataBytes = binaryReader.ReadBytes(numberOfPixels * header.BytesPerPixel);

            for (var pixel = 0; pixel < numberOfPixels; ++pixel)
            {
                var y = (pixel) / header.Width;
                var x = (pixel + 1) % header.Width;

                var indexBytes = new byte[4];

                Array.Copy(
                    pixelDataBytes, 
                    pixel * header.BytesPerPixel, 
                    indexBytes, 
                    0,
                    header.BytesPerPixel);

                var index = BitConverter.ToInt32(indexBytes, 0);
                var color = paletteData[index];

                if (color.A == 0xFE)
                {
                    color = Color.FromArgb(referenceAlpha, color.R, color.G, color.B);
                }

                bitmap.SetPixel(x, y, color);
            }

            return bitmap;
        }


        private static List<Color> ParsePaletteData(IReadOnlyList<byte> data)
        {
            var paletteData = new List<Color>();
            var dataLength = data.Count;
            for (var i = 0; i < dataLength; i += 4)
            {
                paletteData.Add(
                    Color.FromArgb(
                        data[i + 3], 
                        data[i + 2], 
                        data[i + 1], 
                        data[i]));
            }

            return paletteData;
        }

        public static Bitmap ToBitmap(byte[] texData)
        {
            using (var ms = new MemoryStream(texData))
            {
                return ToBitmap(ms);
            }
        }

        #region
        public static void ToGif(Stream texData, Stream output)
        {
            ToBitmap(texData).Save(output, ImageFormat.Gif);
        }

        public static void ToGif(byte[] texData, Stream output)
        {
            ToBitmap(texData).Save(output, ImageFormat.Gif);
        }

        public static void ToJpeg(Stream texData, Stream output)
        {
            ToBitmap(texData).Save(output, ImageFormat.Jpeg);
        }

        public static void ToJpeg(byte[] texData, Stream output)
        {
            ToBitmap(texData).Save(output, ImageFormat.Jpeg);
        }

        public static void ToPng(Stream texData, Stream output)
        {
            ReadTexImage(texData).Save(output, ImageFormat.Png);
        }

        public static void ToPng(byte[] texData, Stream output)
        {
            ToBitmap(texData).Save(output, ImageFormat.Png);
        }
        #endregion

        #endregion Public Methods

        #region Private Methods

        private static bool loadTex(
            ref Bitmap bitmap,
            IReadOnlyList<PaletteColor> palettes,
            uint width,
            uint height,
            bool colorKeyFlag,
            IReadOnlyList<int> palletIndexList)
        {
            if (palettes.Count <= 0)
            {
                return false;
            }

            var pixel = 0;
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    //Tuple.Create((byte)0, (byte)0, (byte)0, (byte)0);
                    PaletteColor palette;
                    if (colorKeyFlag && palletIndexList.Count > 0
                        || palettes.Count < (height * width)
                        && height * width == palletIndexList.Count)
                    {
                        var palletIndex = palletIndexList[pixel];
                        palette = palettes[palletIndex];
                        ++pixel;
                    }
                    else
                    {
                        palette = palettes[pixel];
                        ++pixel;
                    }
                    var color = Color.FromArgb(palette.A, palette.R, palette.G, palette.B);
                    bitmap.SetPixel(x, y, color);
                }
            }
            return true;
        }

        private static Bitmap ReadTexImage(Stream texData)
        {
            Bitmap bitmap;
            var numArray1 = new int[59];
            using (var binaryReader = new BinaryReader(texData))
            {
                for (var index = 0; index <= numArray1.Length - 1; ++index)
                {
                    numArray1[index] = binaryReader.ReadInt32();
                }

                var num1 = numArray1[2];
                var num2 = numArray1[12];
                var num3 = numArray1[13];
                var num4 = numArray1[46];
                var num5 = numArray1[14];
                var width = numArray1[15];
                var height = numArray1[16];
                var num6 = numArray1[19];
                var num7 = numArray1[20];
                var num8 = numArray1[22];
                var num9 = numArray1[25];
                var num10 = numArray1[31];
                var num11 = numArray1[32];
                var num12 = numArray1[33];
                var num13 = numArray1[34];
                bitmap = new Bitmap(width, height);
                var palletIndexList = new List<int>();
                var palettes = new List<PaletteColor>();
                if (num2 > 0 && num6 > 0)
                {
                    for (var index = 0; index < num3; ++index)
                    {
                        if (num5 == 16)
                        {
                            var num14 = (int)binaryReader.ReadUInt16();
                            var colorBytes = new byte[4]
                            {
                                (byte) ((uint) (byte) ((num14 & num10) >> 10) << 3),
                                (byte) ((uint) (byte) ((num14 & num11) >> 5) << 3),
                                (byte) ((uint) (byte) (num14 & num12) << 3),
                                (byte) ((uint) (byte) ((num14 & num13) >> 15) << 7)
                            };
                            var num15 = colorBytes[3] == (byte)254 ? (byte)numArray1[49] : colorBytes[3];
                            var tuple = new PaletteColor { R = colorBytes[2], G = colorBytes[1], B = colorBytes[0], A = num15 };
                            palettes.Add(tuple);
                        }
                        else if (num9 == 24)
                        {
                            var colorBytes = new byte[3]
                            {
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte()
                            };
                            var num14 = num1 <= 0 || colorBytes[0] != (byte)0 || (colorBytes[1] != (byte)0 || colorBytes[2] != (byte)0) ? byte.MaxValue : (byte)0;
                            var tuple = new PaletteColor { R = colorBytes[2], G = colorBytes[1], B = colorBytes[0], A = num14 };
                            palettes.Add(tuple);
                        }
                        else
                        {
                            var colorBytes = new byte[4]
                            {
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte()
                            };
                            var num14 = colorBytes[3] == (byte)254 ? (byte)numArray1[49] : colorBytes[3];
                            var tuple = new PaletteColor { R = colorBytes[2], G = colorBytes[1], B = colorBytes[0], A = num14 };
                            palettes.Add(tuple);
                        }
                    }
                }
                else
                {
                    var imagePixelCount = height * width;
                    for (var index = 0; index < imagePixelCount; ++index)
                    {
                        if (num5 == 16)
                        {
                            var num15 = (int)binaryReader.ReadUInt16();
                            var numArray2 = new byte[4]
                            {
                                (byte) ((uint) (byte) ((num15 & num10) >> 10) << 3),
                                (byte) ((uint) (byte) ((num15 & num11) >> 5) << 3),
                                (byte) ((uint) (byte) (num15 & num12) << 3),
                                (byte) ((uint) (byte) ((num15 & num13) >> 15) << 7)
                            };
                            var num16 = numArray2[3] == (byte)254 ? (byte)numArray1[49] : numArray2[3];
                            var palette = new PaletteColor { R = numArray2[2], G = numArray2[1], B = numArray2[0], A = num16 };
                            palettes.Add(palette);
                        }
                        else if (num9 == 24)
                        {
                            var numArray2 = new byte[3]
                            {
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte()
                            };
                            var num15 = num1 <= 0 || numArray2[0] != (byte)0 || (numArray2[1] != (byte)0 || numArray2[2] != (byte)0) ? byte.MaxValue : (byte)0;
                            var tuple = new PaletteColor { R = numArray2[2], G = numArray2[1], B = numArray2[0], A = num15 };
                            palettes.Add(tuple);
                        }
                        else
                        {
                            var numArray2 = new byte[4]
                            {
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte(),
                                binaryReader.ReadByte()
                            };
                            var palette = new PaletteColor { R = numArray2[2], G = numArray2[1], B = numArray2[0], A = numArray2[3] };
                            palettes.Add(palette);
                        }
                    }
                }
                if (num2 > 0 && num6 > 0)
                {
                    for (var index = 0; index < height * width; ++index)
                    {
                        int num14;
                        switch (num7)
                        {
                            case 16:
                                num14 = (int)binaryReader.ReadUInt16();
                                break;

                            case 32:
                                num14 = binaryReader.ReadInt32();
                                break;

                            default:
                                num14 = (int)binaryReader.ReadByte();
                                break;
                        }
                        palletIndexList.Add(num14);
                    }
                }
                loadTex(ref bitmap, palettes, (uint)width, (uint)height, num1 > 0, palletIndexList);
            }

            return bitmap;
        }

        #endregion Private Methods

        #region Private Structs

        private struct PaletteColor
        {
            #region Public Properties

            public byte A { get; set; }
            public byte B { get; set; }
            public byte G { get; set; }
            public byte R { get; set; }

            #endregion Public Properties
        }

        #endregion Private Structs
    }

    public class TexFileHeader
    {
        public int Version { get; set; }
        public int ColorKeyFlag { get; set; }
        public int MinimumBitsPerColor { get; set; }
        public int MaximumBitsPerColor { get; set; }
        public int MinimumAlphaBits { get; set; }
        public int MaximumAlphaBits { get; set; }
        public int MinimumBitsPerPixel { get; set; }
        public int MaximumBitsPerPixel { get; set; }
        public int NumberOfPalettes { get; set; }
        public int NumberOfColorsPerPalette { get; set; }
        public int BitDepth { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BytesPerRow { get; set; }
        public int PaletteFlag { get; set; }
        public int BitsPerIndex { get; set; }
        public int PaletteSize { get; set; }
        public int BitsPerPixel { get; set; }
        public int BytesPerPixel { get; set; }

        public const int HeaderLength = 0x6C;

        public TexFileHeader(byte[] data)
        {
            if (data.Length < HeaderLength)
            {
                throw new ArgumentException("Not enough data to form correct header", nameof(data));
            }

            this.Version = BitConverter.ToInt32(data, 0x0);
            this.ColorKeyFlag = BitConverter.ToInt32(data, 0x8);
            this.MinimumBitsPerColor = BitConverter.ToInt32(data, 0x14);
            this.MaximumBitsPerColor = BitConverter.ToInt32(data, 0x18);
            this.MinimumAlphaBits = BitConverter.ToInt32(data, 0x1C);
            this.MaximumAlphaBits = BitConverter.ToInt32(data, 0x20);
            this.MinimumBitsPerPixel = BitConverter.ToInt32(data, 0x24);
            this.MaximumBitsPerPixel = BitConverter.ToInt32(data, 0x28);
            this.NumberOfPalettes = BitConverter.ToInt32(data, 0x30);
            this.NumberOfColorsPerPalette = BitConverter.ToInt32(data, 0x34);
            this.BitDepth = BitConverter.ToInt32(data, 0x38);
            this.Width = BitConverter.ToInt32(data, 0x3C);
            this.Height = BitConverter.ToInt32(data, 0x40);
            this.BytesPerRow = BitConverter.ToInt32(data, 0x44);
            this.PaletteFlag = BitConverter.ToInt32(data, 0x4C);
            this.BitsPerIndex = BitConverter.ToInt32(data, 0x50);
            this.PaletteSize = BitConverter.ToInt32(data, 0x58);
            this.BitsPerPixel = BitConverter.ToInt32(data, 0x64);
            this.BytesPerPixel = BitConverter.ToInt32(data, 0x68);
        }
    }

    public class TexFilePixelFormat
    {
        public const int FormatLength = 0x50;
        public TexFilePixelFormat(byte[] data)
        {
            
        }
    }
}