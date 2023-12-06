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
            PluginsWithPosters = Directory.GetDirectories(Paths.PluginPath, "*", SearchOption.TopDirectoryOnly)
                .Select(path => Path.Combine(path, PluginInfo.PLUGIN_NAME))
                .Where(Directory.Exists)
                .Select(PluginWithPosters.FromPluginLethalPostersFolder)
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
        
        Patches.Init(this, Logger);
        ConfigBinder.Init(this, Logger);

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll(typeof(Patches));
        Logger.LogInfo("Loaded patches.");
        
        Logger.LogInfo("Done!.");
    }

    private string[] LoadPostersFromPlugin(PluginWithPosters plugin)
    {
        try
        {
            return Directory.GetFiles(plugin.PostersFolderPath(), "*.png");
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
            return Directory.GetFiles(plugin.TipsFolderPath(), "*.png");
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
        public ConfigEntry<bool> AvailabilityConfigEntry;
        public string[] PostersFiles { get; private set; }
        public string[] TipsFiles { get; private set; }

        public PluginWithPosters(string pluginName)
        {
            PluginName = pluginName;
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
            var pluginName = pluginLethalPostersFolder.Split(Path.DirectorySeparatorChar)[^2];
            return new PluginWithPosters(pluginName);
        }

        public string PostersFolderPath() => Path.Combine(Paths.PluginPath, PluginName, PluginInfo.PLUGIN_NAME, "posters");
        public string TipsFolderPath() => Path.Combine(Paths.PluginPath, PluginName, PluginInfo.PLUGIN_NAME, "tips");
        public bool IsEnabled() => AvailabilityConfigEntry.Value;
    }
}