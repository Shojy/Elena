using System;
using System.IO;

namespace Shojy.FF7.Elena.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //    var path =
            //        @"C:\Program Files (x86)\Steam\steamapps\common\FINAL FANTASY VII\data\lang-en\kernel\";
            //var reader = new KernelReader(Path.Combine(path, "KERNEL.BIN"))
            //    .MergeKernel2Data(Path.Combine(path, "kernel2.bin"));


            var lgp = new LgpReader(@"C:\Program Files (x86)\Steam\steamapps\common\FINAL FANTASY VII\data\menu\menu_us.lgp");

            foreach (var listFile in lgp.ListFiles())
            {
                Console.WriteLine(listFile);
            }

            using (var dataStream = lgp.ExtractFile("yufi.tex"))
            {
                // Save or convert file
            }
            Console.Write("Press any key to exit... ");
            Console.ReadKey();

        }
    }
}
