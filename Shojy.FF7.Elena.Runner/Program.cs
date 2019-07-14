using System;

namespace Shojy.FF7.Elena.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var path =
                @"C:\Program Files (x86)\Steam\steamapps\common\FINAL FANTASY VII\data\lang-en\kernel\KERNEL.BIN";
            var reader = new KernelReader(path);

            Console.Write("Press any key to exit... ");
            Console.ReadKey();

        }
    }
}
