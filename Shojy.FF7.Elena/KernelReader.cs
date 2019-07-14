using System;
using System.IO;
using System.IO.Compression;

namespace Shojy.FF7.Elena
{
    public class KernelReader
    {
        private byte[] _kernelData;
        private KernelType _kernelFile;
        public KernelReader(string filePath, KernelType kernelFile = KernelType.KernelBin)
        {
            this._kernelFile = kernelFile;
            // Load file and decompress
            this._kernelData = Decompress(filePath);

            // Sections

            /*
            #   Section Name                    Start
            1	Command data                    0x0006
            2	Attack data                     0x0086
            3	Battle and growth data          0x063A
            4	Initialization data             0x0F7F
            5	Item data                       0x111B
            6	Weapon data                     0x137A
            7	Armor data                      0x1A30
            8	Accessory data                  0x1B73
            9	Materia data                    0x1C11
            10	Command descriptions	        0x1F32
            11	Magic descriptions  	        0x2199
            12	Item descriptions	            0x28D4
            13	Weapon descriptions	            0x2EE2
            14	Armor descriptions	            0x307B
            15	Accessory descriptions	        0x315F
            16	Materia descriptions	        0x3384
            17	Key Item descriptions	        0x3838
            18	Command Names	                0x3BE2
            19	Magic Names	                    0x3CCA
            20	Item Names	                    0x4293
            21	Weapon Names	                0x4651
            22	Armor Names	                    0x4B02
            23	Accessory Names	                0x4C4B
            24	Materia Names	                0x4D90
            25	Key Item Names	                0x5040
            26	Battle and Battle-Screen Text	0x5217
            27	Summon Attack Names	            0x5692
             */
        }



        private static byte[] Decompress(string path)
        {
            var kernelFile = new FileInfo(path);

            using(var fileStream = kernelFile.OpenRead())
            using(var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            using(var memoryStream = new MemoryStream())
            {
                gzipStream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                return bytes;
            }
        }
    }
}
