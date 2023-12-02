using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace LethalPosters;

internal class Config
{
    private static ConfigFile ConfigFile { get; set; }

    static Config()
    {
        ConfigFile = new ConfigFile(Paths.ConfigPath + "\\LethalPosters.cfg", true);

        foreach (var mod in Plugin.PosterFolders)
        {
            var startIdx = mod.IndexOf(@"plugins\", StringComparison.Ordinal) + @"plugins\".Length;
            var endIdx = mod.IndexOf(@"\LethalPosters", startIdx, StringComparison.Ordinal); 
            var result = mod.Substring(startIdx, endIdx - startIdx);
            
            var conf = ConfigFile.Bind(result, "Enabled", true, $"Enable or disable {result}");
            if (!conf.Value)
            {
                Directory.Move(mod, mod + ".Disabled");
            }
            else
            {
                
            }
        }
    }
}