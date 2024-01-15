using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Random = System.Random;

namespace LethalPosters;

internal class Patches
{
    private static Plugin _plugin;
    private static ManualLogSource _logger;
    private static Random _randomSource;
    
    public static void Init(Plugin plugin, ManualLogSource logger)
    {
        _plugin = plugin;
        _logger = logger;
    }
    
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    [HarmonyPostfix]
    private static void StartPatch()
    {
        _logger.LogInfo("StartOfRound triggered poster refresh");
        UpdateMaterials(0);
    }
    
    [HarmonyPatch(typeof(RoundManager), "GenerateNewLevelClientRpc")]
    [HarmonyPostfix]
    private static void GenerateNewLevelClientRpcPatch(int randomSeed)
    {
        _logger.LogInfo("GenerateNewLevelClientRpc triggered poster refresh");
        UpdateMaterials(randomSeed);
    }

    private static void UpdateMaterials(int seed)
    {
        _logger.LogInfo("Updating textures...");

        _randomSource = new Random(seed);
        
        var materials = GameObject.Find("HangarShip/Plane.001").GetComponent<MeshRenderer>().materials;
        
        UpdateTexture(_plugin.PosterFiles, materials[0]);
        UpdateTexture(_plugin.TipFiles, materials[1]);
    }
    
    private static void UpdateTexture(IEnumerable<string> files, Material material)
    {
        var filesArray = files as string[] ?? files.ToArray();
        if (filesArray.Length == 0)
        {
            _logger.LogWarning($"Tried to update {material.name} texture but had no source files to choose from!");
            return;
        }
        
        var index = _randomSource.Next(filesArray.Length);
        
        var texture = new Texture2D(2, 2);
        _logger.LogInfo($"Updating {material.name} with {filesArray[index]}");
        texture.LoadImage(File.ReadAllBytes(filesArray[index]));
        
        material.mainTexture = texture;
    }
}