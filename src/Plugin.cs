using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace LethalPosters;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public PluginWithPosters[] PluginsWithPosters { get; private set; } = { };

    public IEnumerable<string> PosterFiles => PluginsWithPosters
        .Where(plugin => plugin.IsEnabled())
        .SelectMany(plugin => plugin.PostersFiles);
    
    public IEnumerable<string> TipFiles => PluginsWithPosters
        .Where(plugin => plugin.IsEnabled())
        .SelectMany(plugin => plugin.TipsFiles);
    
    private void Awake()
    {
        Logger.LogInfo("Hello, world!");

        try
        {
            PluginsWithPosters = Directory.GetDirectories(Paths.PluginPath, PluginInfo.PLUGIN_NAME, SearchOption.AllDirectories)
                .Select(PluginWithPosters.FromPluginLethalPostersFolder)
                .ToArray();
            Logger.LogInfo("Discovered poster folders.");
        }
        catch (IOException)
        {
            Logger.LogWarning("Couldn't find any posters folders due to an error.");
            return;
        }
        
        Patches.Init(this, Logger);
        ConfigBinder.Init(this, Logger);
        
        PluginsWithPosters.Do(plugin => plugin.CachePosters(this));
        Logger.LogInfo("Loaded posters.");
        
        PluginsWithPosters.Do(plugin => plugin.CacheTips(this));
        Logger.LogInfo("Loaded tips.");

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll(typeof(Patches));
        Logger.LogInfo("Loaded patches.");
        
        Logger.LogInfo("Done!.");
    }

    private bool IsImageFile(string filePath)
    {
        return Path.GetExtension(filePath).ToLower() switch
        {
            ".bmp" => true,
            ".exr" => true,
            ".gif" => true,
            ".hdr" => true,
            ".iff" => true,
            ".jpg" => true,
            ".jpeg" => true,
            ".pict" => true,
            ".png" => true,
            ".psd" => true,
            ".tga" => true,
            ".tiff" => true,
            _ => false,
        };
    }

    private string[] LoadPostersFromPlugin(PluginWithPosters plugin)
    {
        try
        {
            return Directory.GetFiles(plugin.PostersFolderPath(), "*")
                .Where(IsImageFile)
                .ToArray();
        }
        catch (IOException exception)
        {
            switch (exception)
            {
                case DirectoryNotFoundException:
                    break;
                default:
                    Logger.LogWarning($"Couldn't load posters for {plugin.PluginName}");
                    break;
            }
        }
        return new string[] { };
    }

    private string[] LoadTipsFromPlugin(PluginWithPosters plugin)
    {
        try
        {
            return Directory.GetFiles(plugin.TipsFolderPath(), "*")
                .Where(IsImageFile)
                .ToArray();
        }
        catch (IOException exception)
        {
            switch (exception)
            {
                case DirectoryNotFoundException:
                    break;
                default:
                    Logger.LogWarning($"Couldn't load tips for {plugin.PluginName}");
                    break;
            }
        }
        return new string[] { };
    }

    public class PluginWithPosters
    {
        public string PluginName { get; }
        public string LethalPostersFolder { get; }
        public ConfigEntry<bool> AvailabilityConfigEntry;
        public string[] PostersFiles { get; private set; }
        public string[] TipsFiles { get; private set; }

        public PluginWithPosters(string pluginName, string lethalPostersFolder)
        {
            PluginName = pluginName;
            LethalPostersFolder = lethalPostersFolder;
        }

        public void CachePosters(Plugin plugin)
        {
            PostersFiles = plugin.LoadPostersFromPlugin(this);
        }

        public void CacheTips(Plugin plugin)
        {
            TipsFiles = plugin.LoadTipsFromPlugin(this);
        }

        public static PluginWithPosters FromPluginLethalPostersFolder(string pluginLethalPostersFolder)
        {
            var pathSegments = pluginLethalPostersFolder.Split(Path.DirectorySeparatorChar);
            var pluginSegmentIndex = Array.IndexOf(pathSegments, "plugin");
            var pluginName = pluginSegmentIndex == -1 ? null : pathSegments[pluginSegmentIndex + 1];
            
            return new PluginWithPosters(pluginName, pluginLethalPostersFolder);
        }

        public string PostersFolderPath() => Path.Combine(LethalPostersFolder, PluginInfo.PLUGIN_NAME, "posters");
        public string TipsFolderPath() => Path.Combine(LethalPostersFolder, PluginInfo.PLUGIN_NAME, "tips");
        public bool IsEnabled() => AvailabilityConfigEntry.Value;
    }
}