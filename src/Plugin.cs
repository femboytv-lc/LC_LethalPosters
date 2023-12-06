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
        Logger.LogInfo("Hello, world!");

        try
        {
            PosterFolders = Directory.GetDirectories(Paths.PluginPath, "*", SearchOption.TopDirectoryOnly)
                .Select(path => Path.Combine(path, PluginInfo.PLUGIN_NAME))
                .Where(Directory.Exists)
                .ToArray();
            Logger.LogInfo("Discovered poster folders.");
        }
        catch (IOException)
        {
            Logger.LogWarning("Couldn't find any posters folders due to an error.");
        }
        
        PosterFolders
            .Select(path => Path.Combine(path, "posters"))
            .Do(LoadPostersFromPluginPostersFolder);
        Logger.LogInfo("Loaded posters.");
        
        PosterFolders
            .Select(path => Path.Combine(path, "tips"))
            .Do(LoadTipsFromPluginTipsFolder);
        Logger.LogInfo("Loaded tips.");

        Patches.Init(Logger);

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll(typeof(Patches));
            
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_VERSION}) is loaded!");
    }

    private void LoadPostersFromPluginPostersFolder(string pluginPostersFolderPath)
    {
        try
        {
            Directory.GetFiles(pluginPostersFolderPath, "*.png")
                .Do(PosterFiles.Add);
        }
        catch (IOException exception)
        {
            switch (exception)
            {
                case DirectoryNotFoundException:
                    break;
                default:
                    Logger.LogWarning($"Couldn't load posters for {pluginPostersFolderPath.Split(Path.DirectorySeparatorChar)[^2]}");
                    break;
            }
        }
    }

    private void LoadTipsFromPluginTipsFolder(string pluginTipsFolderPath)
    {
        try
        {
            Directory.GetFiles(pluginTipsFolderPath, "*.png")
                .Do(TipFiles.Add);
        }
        catch (IOException exception)
        {
            switch (exception)
            {
                case DirectoryNotFoundException:
                    break;
                default:
                    Logger.LogWarning($"Couldn't load tips for {pluginTipsFolderPath.Split(Path.DirectorySeparatorChar)[^2]}");
                    break;
            }
        }
    }

    public static string[] PosterFolders { get; private set; } = { };
    public static readonly List<string> PosterFiles = new();
    public static readonly List<string> TipFiles = new();
    public static Random Rand = new();
}