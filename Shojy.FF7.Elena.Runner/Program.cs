using System;
using System.IO;

namespace Shojy.FF7.Elena.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var path =
                @"C:\Program Files (x86)\Steam\steamapps\common\FINAL FANTASY VII\data\lang-en\kernel\";
            var reader = new KernelReader(Path.Combine(path, "KERNEL.BIN"), KernelType.KernelBin)
                .MergeKernel2Data(Path.Combine(path, "kernel2.bin"));

            Console.Write("Press any key to exit... ");
            Console.ReadKey();

        }
    }
}
