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
    
    [HarmonyPatch(typeof(StartOfRound), "StartGame")] // Other function
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    [HarmonyPrefix]
    private static void StartGamePatch()
    {
        Logger.LogInfo("Patching the textures");

        var currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var offset = currentTime % (2 * 60);
        var seed = currentTime - offset;
        
        Logger.LogInfo($"Seed: {seed}, Current time: {currentTime}, Time offset: {offset}");
        
        Random.InitState(seed);
        
        var materials = GameObject.Find("Plane.001").GetComponent<MeshRenderer>().materials;
        
        UpdateTexture(Plugin.PosterFiles, materials[0]);
        UpdateTexture(Plugin.TipFiles, materials[1]);
    }
    
    private static void UpdateTexture(IReadOnlyList<string> files, Material material)
    {
        if (files.Count == 0) {return;}
        
        var index = Random.RandomRangeInt(0, files.Count);
        
        var texture = new Texture2D(2, 2);
        Logger.LogInfo($"Patching {material.name} with {files[index]}");
        texture.LoadImage(File.ReadAllBytes(files[index]));
        
        material.mainTexture = texture;
    }
}