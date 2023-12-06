using System.Collections.Generic;
using System.IO;
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
    
    private static void UpdateTexture(IReadOnlyList<string> files, Material material)
    {
        if (files.Count == 0) {return;}
        
        var index = _randomSource.Next(files.Count);
        
        
        var texture = new Texture2D(2, 2);
        _logger.LogInfo($"Updating {material.name} with {files[index]}");
        texture.LoadImage(File.ReadAllBytes(files[index]));
        
        material.mainTexture = texture;
    }
}