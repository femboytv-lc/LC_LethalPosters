using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LethalPosters.Patches;

internal class StartOfRoundPatches
{
    private static ManualLogSource Logger { get; set; }
    
    public static void Init(ManualLogSource logger)
    {
        Logger = logger;
    }
    
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    [HarmonyPostfix]
    private static void StartPatch()
    {
        Logger.LogInfo("Patching Start in StartOfRound");

        UpdateMaterials(0);
    }
    
    [HarmonyPatch(typeof(RoundManager), "GenerateNewLevelClientRpc")]
    [HarmonyPostfix]
    private static void GenerateNewLevelClientRpcPatch(int randomSeed)
    {
        Logger.LogInfo("Patching GenerateNewLevelClientRpc in RoundManager");

        UpdateMaterials(randomSeed);
    }

    private static void UpdateMaterials(int seed)
    {
        Logger.LogInfo("Patching the textures");

        Plugin.Rand = new System.Random(seed);
        
        var materials = GameObject.Find("Plane.001").GetComponent<MeshRenderer>().materials;
        
        UpdateTexture(Plugin.PosterFiles, materials[0]);
        UpdateTexture(Plugin.TipFiles, materials[1]);
    }
    
    private static void UpdateTexture(IReadOnlyList<string> files, Material material)
    {
        if (files.Count == 0) {return;}
        
        var index = Plugin.Rand.Next(files.Count);
        
        var texture = new Texture2D(2, 2);
        Logger.LogInfo($"Patching {material.name} with {files[index]}");
        texture.LoadImage(File.ReadAllBytes(files[index]));
        
        material.mainTexture = texture;
    }
}