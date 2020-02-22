using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using _7thHeaven;
using _7thWrapperLib;
using Iros._7th.Workshop;
using SeventhHeaven.Classes;

namespace Shojy.FF7.Elena.Mod.Helpers
{
    public static class ModExtensions
    {
        private static RuntimeProfile _cacheProfile = null;

        public static RuntimeProfile GetRuntimeProfile(this _7HPlugin plugin)
        {
            if (_cacheProfile != null)
            {
                return _cacheProfile;
            }

            MethodInfo dynMethod = typeof(GameLauncher).GetMethod("CreateRuntimeProfile",
                BindingFlags.NonPublic | BindingFlags.Static);

            var profile = (RuntimeProfile)dynMethod?.Invoke(plugin, null);

            if (profile is null)
            {
                throw new ApplicationException("Could not load RuntimeProfile instance");
            }

            foreach (var mod in profile.Mods)
            {
                mod.Startup();
            }

            _cacheProfile = profile;

            return profile;
        }

        public static IrosArc GetArchive(this RuntimeMod mod)
        {
            //var archive = mod.GetType().GetField("_archive", (BindingFlags)0x01FFFFFF);
            //if (archive != null)
            //{
            //    var value = (IrosArc)archive.GetValue(mod);

            //    return value;
            //}

            if (mod.BaseFolder.EndsWith(".iro", StringComparison.InvariantCultureIgnoreCase))
            {
                return new IrosArc(mod.BaseFolder);
            }

            return null;
        }

        public static IrosArc CopyFileToOutput(this IrosArc archive, string file, string output)
        {
            var stream = EnsureClean(output);

            return archive.CopyFileToStream(file, stream.BaseStream);
        }

        public static IrosArc CopyFileToStream(this IrosArc archive, string file, Stream output)
        {
            using var input = archive.GetData(file);
            
            input.CopyTo(output);

            return archive;
        }

        public static void CopyFileToOutput(this RuntimeMod mod, string filename, string output)
        {
            using var file = mod.Read(filename);
            using var stream = EnsureClean(output);
            file.CopyTo(stream.BaseStream);
        }
        
        public static StreamWriter EnsureClean(string file)
        {
            var path = Path.GetDirectoryName(file);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            return new StreamWriter(file);
        }
    }
}
