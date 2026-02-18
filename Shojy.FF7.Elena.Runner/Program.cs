using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using Shojy.FF7.Elena.Converters;
using Shojy.FF7.Elena.Text;

namespace Shojy.FF7.Elena.Runner
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting demo of Elena functionality. Please ensure FF7 is running...");
            Process ff7 = null;
            do
            {
                try
                {
                    ff7 ??= Process.GetProcessesByName("ff7_en").FirstOrDefault() ?? Process.GetProcessesByName("ff7").FirstOrDefault();
                }
                catch 
                {
                    // Wait and try again
                    Thread.Sleep(250);
                }

            } while (ff7?.MainModule is null);

            var ff7Folder = Path.GetDirectoryName(ff7.MainModule.FileName);

            var path = Path.Combine(ff7Folder, "data", "lang-en", "kernel");
            var minigame = Path.Combine(ff7Folder, "data", "minigame");

            // Read Kernel Data
            var reader = new KernelReader(Path.Combine(path, "KERNEL.BIN"))
                .MergeKernel2Data(Path.Combine(path, "kernel2.bin"));

            var weapons = reader.WeaponData.Weapons;

            foreach (var wpn in weapons)
            {
                Console.WriteLine($"Found a {wpn.Name}!");
            }

            // Read battle text
            Console.WriteLine("Reading battle text...");
            var btlText = reader.BattleText.Strings;

            foreach (var btl in btlText)
            {
                Console.WriteLine(btl);
                Console.WriteLine($"Bytes: {BitConverter.ToString(btl.GetBytes())}");
            }

            //  Test the alert command
            var test = new FFText("{ALERT}This is a test!");
            Console.WriteLine(test);
            Console.WriteLine($"Bytes: {BitConverter.ToString(test.GetBytes())}");

            // This must have a call to MergeKernel2Data for now. The full dataset has not yet been found.
            // Otherwise this will load the data from active memory instead of the kernel file on disk.
            var mem = new MemoryKernelReader(ff7)
                .MergeKernel2Data(Path.Combine(path, "kernel2.bin"));



            var lgp = new LgpReader(Path.Combine(minigame, "chocobo.lgp"));

            Console.WriteLine($"Extracting .tex files as png from chocobo.lgp. Putting in img/");
            Directory.CreateDirectory("img");
            foreach (var listFile in lgp.ListFiles())
            {
                if (!listFile.EndsWith(".tex"))
                {
                    continue;
                }
                Console.WriteLine(listFile);
                using (var dataStream = lgp.ExtractFile(listFile))
                using (var fileWriter = new StreamWriter($"img\\{listFile}.png"))
                {
                    // Save or convert file
                    try
                    {
                        var bmp = TexConverter.ToBitmap(dataStream);

                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height),
                                new Rectangle(0, 0, bmp.Width, bmp.Height),
                                GraphicsUnit.Pixel);
                        }

                        bmp.Save(fileWriter.BaseStream, ImageFormat.Png);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{listFile} errored:\n{ex.Message}");
                    }
                }
            }


            Console.Write("Press any key to exit... ");
            Console.ReadKey();

        }
    }
}
 