using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

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
        Logger.LogInfo("Patched Start in StartOfRound");
        
        UpdateMaterial(0);
    }
    
    [HarmonyPatch(typeof(StartOfRound), "StartGame")]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void StartGamePatch(StartOfRound __instance)
    {
        Logger.LogInfo("Patched StartGame in StartOfRound");
        
        UpdateMaterial(__instance.randomMapSeed);
    }

    private static void UpdateMaterial(int seed)
    {
        Logger.LogInfo("Patching the textures");
        
        var rand = new System.Random(seed);

        var materials = GameObject.Find("Plane.001").GetComponent<MeshRenderer>().materials;

        UpdateTexture(rand, Plugin.PosterFiles, materials[0]);
        UpdateTexture(rand, Plugin.TipFiles, materials[1]);
    }
    
    private static void UpdateTexture(System.Random rand, IReadOnlyList<string> files, Material material)
    {
        if (files.Count == 0) {return;}
        
        var index = rand.Next(files.Count);

        var texture = new Texture2D(2, 2);
        Logger.LogInfo($"Patching {material.name} with {files[index]}");
        texture.LoadImage(File.ReadAllBytes(files[index]));
        
        material.mainTexture = texture;
    }
}