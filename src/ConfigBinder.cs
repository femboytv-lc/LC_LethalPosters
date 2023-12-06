using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using Logger = HarmonyLib.Tools.Logger;

namespace LethalPosters;

internal class ConfigBinder
{
    private static Plugin _plugin;
    private static ManualLogSource _logger;
    
    public static void Init(Plugin plugin, ManualLogSource logger)
    {
        _plugin = plugin;
        _logger = logger;
        _plugin.PluginsWithPosters.Do(BindExternalPluginConfigEntry);
        _logger.LogInfo("Finishing binding ConfigEntries.");
    }

    static void BindExternalPluginConfigEntry(Plugin.PluginWithPosters plugin)
    {
        if (plugin.PluginName is null) return;
        _logger.LogInfo($"Binding ConfigEntry for {plugin.PluginName}...");
        plugin.AvailabilityConfigEntry = _plugin.Config.Bind(plugin.PluginName, "Enabled", true, $"Enable or disable {plugin.PluginName}");
    }
}