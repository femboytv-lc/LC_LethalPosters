using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace LethalPosters
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            var folders = Directory.GetDirectories(Paths.PluginPath, PluginInfo.PLUGIN_NAME, SearchOption.AllDirectories).ToList();

            foreach (var folder in folders)
            {
                foreach (var file in Directory.GetFiles(Path.Combine(folder, "posters")))
                {
                    PosterFiles.Add(file);
                }

                foreach (var file in Directory.GetFiles(Path.Combine(folder, "tips")))
                {
                    TipFiles.Add(file);
                }
            }

            Patches.StartOfRoundPatches.Init(Logger);

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(typeof(Patches.StartOfRoundPatches));

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_VERSION}) is loaded!");
        }

        public static readonly List<string> PosterFiles = new();
        public static readonly List<string> TipFiles = new();
        public static System.Random Rand = new();
    }
}
