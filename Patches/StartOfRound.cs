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
    [HarmonyPatch(typeof(StartOfRound), "StartGame")]
    [HarmonyPostfix]
    private static void StartPatch()
    {
        var materials = GameObject.Find("Plane.001").GetComponent<MeshRenderer>().materials;

        UpdateTexture(Plugin.PosterFiles, materials[0]);
        UpdateTexture(Plugin.TipFiles, materials[1]);
    }
    
    private static void UpdateTexture(List<string> files, Material material)
    {
        if (files.Count == 0) {return;}

        var index = Plugin.Rand.Next(files.Count);
        Logger.LogInfo($"1.. {index}");
        foreach (var str in files)
        {
            Logger.LogInfo($"2.. {str}");
        }
        var texture = new Texture2D(2, 2);
        texture.LoadImage(File.ReadAllBytes(files[index]));
        
        material.mainTexture = texture;
    }
}