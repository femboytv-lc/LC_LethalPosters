using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;

namespace LethalPosters
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            PosterFolders = Directory.GetDirectories(Paths.PluginPath, PluginInfo.PLUGIN_NAME, SearchOption.AllDirectories).ToList();
            
            foreach (var folder in PosterFolders)
            {
                foreach (var file in Directory.GetFiles(Path.Combine(folder, "posters")))
                {
                    if (Path.GetExtension(file) != ".old")
                    {
                        PosterFiles.Add(file);
                    }
                }

                foreach (var file in Directory.GetFiles(Path.Combine(folder, "tips")))
                {
                    if (Path.GetExtension(file) != ".old")
                    {
                        TipFiles.Add(file);
                    }
                }
            }

            Patches.Init(Logger);

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(typeof(Patches));
            
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_VERSION}) is loaded!");
        }

        public static List<string> PosterFolders = new();
        public static readonly List<string> PosterFiles = new();
        public static readonly List<string> TipFiles = new();
        public static Random Rand = new();
    }
}
