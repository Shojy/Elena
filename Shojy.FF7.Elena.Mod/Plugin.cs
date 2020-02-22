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
       

        private RuntimeMod ThisMod;
        public override void Start(RuntimeMod thisMod)
        {
            this.ThisMod = thisMod;
            GameLauncher.Instance.LaunchCompleted += this.Instance_LaunchCompleted;
        }

        private void Instance_LaunchCompleted(bool wasSuccessful)
        {
            if (wasSuccessful)
            {
                this.GameLaunched(this.ThisMod);
            }
        }

        public void GameLaunched(RuntimeMod thisMod)
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

            using var jsonWriter = new StreamWriter(@"C:\Games\profile.json");
            jsonWriter.Write(JsonConvert.SerializeObject(this.GetRuntimeProfile(), new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));

            this.interactive7 = Process.Start(Path.Combine(basePath, "InteractiveSeven.exe"));

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
                                        if (ov.File.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            this.ProcessPng(ms, img, basePath);
                                        }
                                        else if (ov.File.EndsWith(".tex", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            // ProcessTex()
                                        }
                                        didFind = true;
                                        break;
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
                                continue;
                            }
                        }
                    }
                }
            }
        }

        private void ProcessPng(Stream img, ImageFile export, string basePath)
        {
            var bmp = Image.FromStream(img);
            
            var crop = new Rectangle(
                (int)(bmp.Width * export.LocationXRatio),
                (int)(bmp.Height * export.LocationYRatio),
                (int)(bmp.Width * export.CropXRatio),
                (int)(bmp.Height * export.CropYRatio));

            var croppedBmp = new Bitmap((int)(bmp.Width * export.CropXRatio), (int)(bmp.Height * export.CropYRatio));

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
            this.interactive7.Close();
        }
    }
}
