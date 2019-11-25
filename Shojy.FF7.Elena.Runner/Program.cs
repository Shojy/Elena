using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using Shojy.FF7.Elena.Converters;

namespace Shojy.FF7.Elena.Runner
{
    class Program
    {
        static void Main()
        {
            Process FF7 = null;
            while (FF7 is null)
            {
                try
                {
                    if (FF7 is null) FF7 = Process.GetProcessesByName("ff7_en").FirstOrDefault();
                    if (FF7 is null) FF7 = Process.GetProcessesByName("ff7").FirstOrDefault();
                }
                catch (Exception e)
                {
                }

                Thread.Sleep(250);
            }

            var path =
                @"D:\Personal\SteamLibrary\steamapps\common\FINAL FANTASY VII\data\lang-en\kernel\";
            var reader = new KernelReader(Path.Combine(path, "KERNEL.BIN"))
                .MergeKernel2Data(Path.Combine(path, "kernel2.bin"));


            // This must have a call to MergeKernel2Data for now. The full dataset has not yet been found.
            var mem = new MemoryKernelReader(FF7)
                .MergeKernel2Data(Path.Combine(path, "kernel2.bin"));



            //var lgp = new LgpReader(@"C:\Users\Shojy\Downloads\Aalis LGP+UNLGP 0.5b Compiled By Kranmer\choco\chocobo.lgp");

            //Directory.CreateDirectory("img");
            //foreach (var listFile in lgp.ListFiles())
            //{
            //    if (!listFile.EndsWith(".tex"))
            //    {
            //        continue;
            //    }
            //    Console.WriteLine(listFile);
            //    using (var dataStream = lgp.ExtractFile(listFile))
            //    using (var fileWriter = new StreamWriter($"img\\{listFile}.png"))
            //    {
            //        // Save or convert file
            //        try
            //        {
            //            var crop = new Rectangle(0, 0, 84, 96);
            //            var bmp = TexConverter.ToBitmap(dataStream);
            //            var croppedBmp = new Bitmap(84, 96);

            //            using (var g = Graphics.FromImage(croppedBmp))
            //            {
            //                g.DrawImage(bmp, new Rectangle(0, 0, croppedBmp.Width, croppedBmp.Height),
            //                    crop,
            //                    GraphicsUnit.Pixel);
            //            }

            //            croppedBmp.Save(fileWriter.BaseStream, ImageFormat.Png);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine($"{listFile} errored:\n{ex.Message}");
            //        }
            //    }
            //}

            
            Console.Write("Press any key to exit... ");
            Console.ReadKey();

        }
    }
}
 