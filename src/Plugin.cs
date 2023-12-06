using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;

namespace LethalPosters;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        PosterFolders = Directory.GetDirectories(Paths.PluginPath, $"*/{PluginInfo.PLUGIN_NAME}");
        
        PosterFolders
            .Select(path => Path.Combine(path, "posters"))
            .Do(LoadPostersFromPluginPostersFolder);
        
        PosterFolders
            .Select(path => Path.Combine(path, "tips"))
            .Do(LoadTipsFromPluginTipsFolder);

        Patches.Init(Logger);

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll(typeof(Patches));
            
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_VERSION}) is loaded!");
    }

    private void LoadPostersFromPluginPostersFolder(string pluginPostersFolderPath)
    {
        foreach (var file in Directory.GetFiles(pluginPostersFolderPath))
        {
            if (Path.GetExtension(file) != ".old")
            {
                PosterFiles.Add(file);
            }
        }
    }

    private void LoadTipsFromPluginTipsFolder(string pluginTipsFolderPath)
    {
        foreach (var file in Directory.GetFiles(pluginTipsFolderPath))
        {
            if (Path.GetExtension(file) != ".old")
            {
                TipFiles.Add(file);
            }
        }
    }
    
    public static string[] PosterFolders { get; private set; }
    public static readonly List<string> PosterFiles = new();
    public static readonly List<string> TipFiles = new();
    public static Random Rand = new();
}