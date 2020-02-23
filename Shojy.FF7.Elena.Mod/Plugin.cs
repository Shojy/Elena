using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using _7thWrapperLib;
using Iros._7th.Workshop;
using Newtonsoft.Json;
using SeventhHeaven.Classes;
using Shojy.FF7.Elena.Mod;
using Shojy.FF7.Elena.Mod.Helpers;

namespace _7thHeaven
{
    public class Plugin : _7HPlugin
    {
        public override void Start(RuntimeMod thisMod)
        {
            var profile = this.GetRuntimeProfile();

            var basePath = Path.Combine(profile.FF7Path, "mods", "Interactive7");

            // Locate own files
            var json = "";
            if (thisMod.BaseFolder.EndsWith(".iro"))
            {
                var arc = new IrosArc(thisMod.BaseFolder);
                var i7Files = arc.AllFileNames().Where(f => f.StartsWith("i7\\"));

                foreach (var file in i7Files)
                {
                    try
                    {
                        var path = Path.Combine(basePath, file.Substring(3, file.Length - 3));
                        var dire = Path.GetDirectoryName(path);

                        if (!Directory.Exists(dire))
                        {
                            Directory.CreateDirectory(dire);
                        }

                        arc.CopyFileToOutput(file, path);
                    }
                    catch(Exception)
                    {
                        // Chances are if this is hit, there's a file in place already open.
                        // Just ignore it for now, and address later if it becomes a problem.
                    }
                }

                using var filemapStream = arc.GetData("filemap.json");
                using var reader = new StreamReader(filemapStream);
                json = reader.ReadToEnd();
            }
            else
            {
                using var filemapStream = File.Open(Path.Combine(thisMod.BaseFolder, "filemap.json"), FileMode.Open);
                using var reader = new StreamReader(filemapStream);
                var i7BasePath = Path.Combine(thisMod.BaseFolder, "i7");
                var i7Files = Directory.EnumerateFiles(i7BasePath, "*", SearchOption.AllDirectories);

                foreach (var file in i7Files)
                {
                    try
                    {
                        var shortPath = file.Substring(i7BasePath.Length, file.Length - i7BasePath.Length);
                        var path = Path.Combine(basePath, shortPath);
                        var dire = Path.GetDirectoryName(path);

                        if (!Directory.Exists(dire))
                        {
                            Directory.CreateDirectory(dire);
                        }
                        File.Copy(file, path, true);
                    }
                    catch (Exception)
                    {
                        // Like above, just ignoring this for now unless it becomes a problem.
                    }
                }

                json = reader.ReadToEnd();
            }


            // Read JSON Map
            var map = JsonConvert.DeserializeObject<TsengFileMap>(json);

            // Extract Json Files
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }


            this.ExtractKernels(map, basePath, profile);

            
            this.ExtractImages(map, basePath, profile);

            if (this.interactive7 is null)
            {
                var proc = new ProcessStartInfo(Path.Combine(basePath, "InteractiveSeven.exe"), "--7h")
                {
                    WorkingDirectory = basePath
                };
                this.interactive7 = Process.Start(proc);
            }

        }

        private Process interactive7;

        private void ExtractKernels(TsengFileMap map, string basePath, RuntimeProfile profile)
        {
            foreach (var kernel in map.Kernels)
            {
                var didFindKernel = false;
                var outFile = Path.Combine(basePath, kernel.Value.OutputFile);
                var path = Path.GetDirectoryName(outFile);

                if (path is null)
                {
                    continue;
                }

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var file = $"kernel\\{kernel.Key}";

                foreach (var mod in profile.Mods)
                {
                    if (mod.HasFile(file))
                    {
                        mod.CopyFileToOutput(file, outFile);
                        didFindKernel = true;
                        break;
                    }

                    var overrides = mod.GetOverrides(file);

                    if (overrides.Any())
                    {
                        var ov = overrides.First();
                        using var outf = ModExtensions.EnsureClean(outFile);

                        var arc = mod.GetArchive();
                        if (arc != null)
                        {
                            arc.GetData(ov.File).CopyTo(outf.BaseStream);
                            didFindKernel = true;
                            break;
                        }
                    }
                }

                if (!didFindKernel)
                {
                    // Get base game data
                    var loc = Path.Combine(profile.FF7Path, "data", "kernel", kernel.Key);
                    if (File.Exists(loc))
                    {
                        File.Copy(loc, outFile, true);
                        continue;
                    }

                    loc = Path.Combine(profile.FF7Path, "data", "lang-en", "kernel");

                    if (File.Exists(loc))
                    {
                        File.Copy(loc, outFile, true);
                        continue;
                    }
                }
            }
        }

        private void ExtractImages(TsengFileMap map, string basePath, RuntimeProfile profile)
        {
            foreach (var lgpSet in map.Images)
            {
                foreach (var tex in lgpSet.Value)
                {
                    foreach (var img in tex.Value)
                    {
                        var didFind = false;

                        foreach (var mod in profile.Mods)
                        {
                            if (didFind) break;
                            foreach (var variant in img.GetPossibleOverrideNames(tex.Key, lgpSet.Key))
                            {

                                var overrides = mod.GetOverrides(variant).ToArray();

                                var ov = overrides.FirstOrDefault();
                                if (ov != null)
                                {
                                    var arc = mod.GetArchive();
                                    if (arc != null)
                                    {
                                        using var ms = new MemoryStream();
                                        arc.GetData(ov.File).CopyTo(ms);
                                        var exts = new[] {".png", ".bmp", ".jpg", ".jpeg", ".gif"};
                                        if (exts.Any(e => ov.File.EndsWith(e, StringComparison.InvariantCultureIgnoreCase)))
                                        {
                                            ProcessPng(ms, img, basePath);
                                        }
                                        else if (ov.File.EndsWith(".tex", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            ProcessTex(ms, img, basePath);
                                        }
                                        didFind = true;
                                        break;
                                    }
                                }
                            }

                            var lgpParts = lgpSet.Key.Split('\\', '/');
                            for (var i = lgpParts.Length - 1; i >= 0; --i)
                            {
                                var path = "";
                                for (var j = i; j < lgpParts.Length; ++j)
                                {
                                    path += lgpParts[j] + "\\";
                                }

                                path = path.TrimEnd('\\');
                                var ov = mod.GetOverrides(path).FirstOrDefault();
                                
                                if (ov != null)
                                {
                                    try
                                    {
                                        var arc = mod.GetArchive();
                                        using var lgp = arc.GetData(path);

                                        var reader = new LgpReader(lgp);

                                        if (reader.ListFiles().Contains(tex.Key))
                                        {
                                            using var texStream = reader.ExtractFile(tex.Key);
                                            ProcessTex(texStream, img, basePath);
                                            didFind = true;
                                            break;
                                        }
                                    }
                                    catch
                                    {
                                        // Skip any invalid or corrupt files.
                                    }
                                }
                            }

                        }

                        if (!didFind)
                        {
                            // Get base game data
                            var loc = Path.Combine(profile.FF7Path, "data", lgpSet.Key);
                            if (File.Exists(loc))
                            {
                                var reader = new LgpReader(loc);

                                try
                                {
                                    if (reader.ListFiles().Contains(tex.Key))
                                    {
                                        using var texStream = reader.ExtractFile(tex.Key);
                                        ProcessTex(texStream, img, basePath);
                                    }
                                }
                                catch
                                {
                                    // Skip corrupted or invalid files
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ProcessTex(Stream img, ImageFile export, string basePath)
        {
            var bmp = new Tex(img).ToBitmap(export.ColorPalette);
            ProcessBitmap(bmp, export, basePath);
        }

        private static void ProcessPng(Stream img, ImageFile export, string basePath)
        {
            var bmp = Image.FromStream(img);

            ProcessBitmap(bmp, export, basePath);
        }

        private static void ProcessBitmap(Image bmp, ImageFile export, string basePath)
        {
            var crop = new Rectangle(
                (int) (bmp.Width * export.LocationXRatio),
                (int) (bmp.Height * export.LocationYRatio),
                (int) (bmp.Width * export.CropXRatio),
                (int) (bmp.Height * export.CropYRatio));

            var croppedBmp = new Bitmap((int) (bmp.Width * export.CropXRatio), (int) (bmp.Height * export.CropYRatio));

            using (var g = Graphics.FromImage(croppedBmp))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, croppedBmp.Width, croppedBmp.Height),
                    crop,
                    GraphicsUnit.Pixel);
            }

            using var writer = ModExtensions.EnsureClean(Path.Combine(basePath, export.OutputFile));

            croppedBmp.Save(writer.BaseStream, ImageFormat.Png);
        }


        public override void Stop()
        {
            this.interactive7.Kill();
            this.interactive7 = null;
        }
    }
}
