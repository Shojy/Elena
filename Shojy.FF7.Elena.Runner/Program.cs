using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Shojy.FF7.Elena.Converters;

namespace Shojy.FF7.Elena.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var path =
                @"C:\Program Files (x86)\Steam\steamapps\common\FINAL FANTASY VII\data\lang-en\kernel\";
            var reader = new KernelReader(Path.Combine(path, "KERNEL.BIN"))
                .MergeKernel2Data(Path.Combine(path, "kernel2.bin"));




            var lgp = new LgpReader(@"C:\Users\Shojy\Downloads\Aalis LGP+UNLGP 0.5b Compiled By Kranmer\choco\chocobo.lgp");

            for (var i = 0; i < 16; i++)
            {
                using (var dataStream = lgp.ExtractFile("bv.tex"))
                {
                    var bmp = TexConverter.ToBitmap(dataStream, i);
                    bmp.Save($"choco-{i}.png");
                }
            }

            Directory.CreateDirectory("img");
            var portraits = new Dictionary<string, string>
                        {
                            {"barret", "barre.tex"},
                            {"vincent", "bins.tex"},
                            {"chocobo", "choco.tex"},
                            {"cid", "cido.tex"},
                            {"cloud", "cloud.tex"},
                            {"aeris", "earith.tex"},
                            {"cait-sith", "ketc.tex"},
                            {"sephiroth", "pcefi.tex"},
                            {"young-cloud", "pcloud.tex"},
                            {"red-xiii", "red.tex"},
                            {"tifa", "tifa.tex"},
                            {"yuffie", "yufi.tex"}
                        };
            const int portraitBaseWidth = 128;
            const int portraitBaseHeight = 128;
            const decimal portraitTargetWidth = 84;
            const decimal portraitTargetHeight = 96;
            const decimal portraitWidthRatio = portraitTargetWidth / portraitBaseWidth;
            const decimal portraitHeightRatio = portraitTargetHeight / portraitBaseHeight;

            foreach (var portrait in portraits)
            {
                Console.WriteLine(portrait);

                using (var dataStream = lgp.ExtractFile(portrait.Value))
                using (var fileWriter = new StreamWriter($"img\\{portrait.Key}.png"))
                {
                    // Save or convert file
                    try
                    {
                        var bmp = TexConverter.ToBitmap(dataStream);

                        var croppedWidth = (int)(bmp.Width * portraitWidthRatio);
                        var croppedHeight = (int)(bmp.Height * portraitHeightRatio);

                        var crop = new Rectangle(0, 0, croppedWidth, croppedHeight);
                        var croppedBmp = new Bitmap(croppedWidth, croppedHeight);

                        using (var g = Graphics.FromImage(croppedBmp))
                        {
                            g.DrawImage(bmp, new Rectangle(0, 0, croppedBmp.Width, croppedBmp.Height),
                                crop,
                                GraphicsUnit.Pixel);
                        }

                        croppedBmp.Save(fileWriter.BaseStream, ImageFormat.Png);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{portrait} errored:\n{ex.Message}");
                    }
                }
            }

            var iconMap = new Dictionary<string, (string name, int x, int y)[]>
            {
                {
                    "btl_win_a_l.tex", new[]
                    {
                        (name: "slot-empty", x: 7, y: 2),
                        (name: "slot-link", x: 7, y: 3),
                        (name: "item", x: 6, y: 7),
                        (name: "sword", x: 7, y: 7),
                    }
                }, {
                    "btl_win_b_l.tex", new[]
                    {
                        /* Materia Indexes:
                         4 - Magic
                         5 - Support
                         8 - Command
                         9 - Independent
                         10 - Summon
                         */
                        (name: "materia", x: 0, y: 2),
                        (name: "slot-normal", x: 0, y: 3),
                    }
                }, {
                    "btl_win_c_l.tex", new[]
                    {
                        (name: "arm", x: 6, y: 0),
                        (name: "glove", x: 7, y: 0),
                        (name: "staff", x: 6, y: 1),
                        (name: "hairpin", x: 7, y: 1),
                        (name: "shuriken", x: 6, y: 2),
                        (name: "megaphone", x: 7, y: 2),
                        (name: "gun", x: 6, y: 3),
                        (name: "pole", x: 7, y: 3),
                        (name: "armlet", x: 6, y: 4),
                        (name: "accessory", x: 7, y: 4),
                    }
                }

            };

            foreach (var map in iconMap)
            {
                for (var i = 0; i < 16; i++)
                {

                    using (var dataStream = lgp.ExtractFile(map.Key))
                    {
                        var bmp = TexConverter.ToBitmap(dataStream, i);

                        var size = bmp.Width / 8;

                        foreach (var (name, x, y) in map.Value)
                        {
                            using (var fileWriter = new StreamWriter($"img\\{i}-{name}.png"))
                            {
                                var croppedWidth = (int)(bmp.Width * portraitWidthRatio);
                                var croppedHeight = (int)(bmp.Height * portraitHeightRatio);

                                var crop = new Rectangle(x * size, y * size, size, size);
                                var croppedBmp = new Bitmap(size, size);

                                using (var g = Graphics.FromImage(croppedBmp))
                                {
                                    g.DrawImage(bmp, new Rectangle(0, 0, croppedBmp.Width, croppedBmp.Height),
                                        crop,
                                        GraphicsUnit.Pixel);
                                }

                                croppedBmp.Save(fileWriter.BaseStream, ImageFormat.Png);
                            }
                        }
                    }
                }
            }

            Console.Write("Press any key to exit... ");
            Console.ReadKey();
        }
    }
}
