using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace LethalPosters
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            TexturesFolder = Directory.GetDirectories(Paths.PluginPath, PluginInfo.PLUGIN_NAME, SearchOption.AllDirectories).ToList<string>();
            PostersFileNames = new List<string>();
            TipsFileNames = new List<string>();

            var postersFiles = Directory.GetFiles(Path.Combine(TexturesFolder, "posters"));
            var tipsFiles = Directory.GetFiles(Path.Combine(TexturesFolder, "tips"));
            
            foreach (var textureFile in postersFiles)
            {
                PostersFileNames.Add(textureFile);
            }
            
            foreach (var textureFile in tipsFiles)
            {
                TipsFileNames.Add(textureFile);
            }
            
            Patches.StartOfRoundPatches.Init(Logger);
            
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(typeof(Patches.StartOfRoundPatches));
            
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_VERSION}) is loaded!");
        }

        public static string TexturesFolder;
        public static List<string> PostersFileNames;
        public static List<string> TipsFileNames;
    }
}
