using System;
using System.Collections.Generic;
using System.IO;

namespace Shojy.FF7.Elena.Mod
{
    public class BaseFile
    {
        public string OutputFile { get; set; }
    }

    public class ImageFile : BaseFile
    {
        public long ColorPalette { get; set; }

        public double LocationXRatio { get; set; }

        public double LocationYRatio { get; set; }

        public double CropXRatio { get; set; }

        public double CropYRatio { get; set; }

        public IEnumerable<string> GetPossibleOverrideNames(string name, string container)
        {
            if (container.EndsWith(".lgp", StringComparison.InvariantCultureIgnoreCase))
            {
                container = container.Substring(0, container.Length - 4);
            }

            var segments = container.Split('\\', '/', '_');

            for (var i = 0; i < segments.Length; ++i)
            {
                var path = string.Empty;

                for (var j = 0; j < i; ++j)
                {
                    path += $"{segments[j]}\\";
                }

               
                yield return $"{path}{name}";

                var parts = name.Split(new[] {'.'}, 2);
                yield return $"{path}{parts[0]}_{this.ColorPalette:00}.png";
                yield return $"{path}{parts[0]}_{this.ColorPalette:00}";
                yield return $"{path}{parts[0]}";

            }

            yield return container;
        }
    }

   

    public class TsengFileMap
    {
        public Dictionary<string, BaseFile> Kernels { get; set; }

        public Dictionary<string, Dictionary<string, ImageFile[]>> Images { get; set; }
    }
}