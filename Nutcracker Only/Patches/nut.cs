using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;


namespace Nutcracker_Only.Patches;

public class Nut
{
    
    private static SelectableLevel previousLevel = new SelectableLevel();
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    [HarmonyPostfix]

    public static void SetWeightOfShells(StartOfRound __instance)
    {
        __instance.allItemsList.itemsList[59].weight = 100;
      

    }

    [HarmonyPatch(typeof(RoundManager), "FinishGeneratingLevel")]
    [HarmonyPostfix]

    public static void SetNutcrackersOnly(RoundManager __instance)
    {
        
        SpawnableEnemyWithRarity Crack = StartOfRound.Instance.levels[1].Enemies[8];
        Crack.rarity = 90;
        SpawnableEnemyWithRarity Bracken  = StartOfRound.Instance.levels[5].Enemies[5];
        Bracken.rarity = 7;
        SpawnableEnemyWithRarity Coil = StartOfRound.Instance.levels[5].Enemies[6];
        Coil.rarity = 2;
        SpawnableEnemyWithRarity GhostGirl = StartOfRound.Instance.levels[5].Enemies[0];
        GhostGirl.rarity = 1;
        
        if (__instance.currentLevel.currentWeather == LevelWeatherType.Eclipsed)
        {
           
            previousLevel = ScriptableObject.CreateInstance<SelectableLevel>();
            previousLevel.levelID = __instance.currentLevel.levelID;
            previousLevel.OutsideEnemies = __instance.currentLevel.OutsideEnemies
                .Select(e => new SpawnableEnemyWithRarity
                {
                    enemyType = e.enemyType,
                    rarity = e.rarity
                }).ToList();

            __instance.currentLevel.OutsideEnemies.Clear();
            __instance.currentLevel.OutsideEnemies.Add(Crack);
            __instance.currentLevel.OutsideEnemies[0].enemyType.MaxCount = 70;
        }
        

        if (__instance.currentLevel.levelID == 1)
        {
            __instance.currentLevel.Enemies.Clear();
            __instance.currentLevel.Enemies.Add(Crack);
            __instance.currentLevel.Enemies[0].enemyType.MaxCount = 30;
            return;
        }
        __instance.currentLevel.Enemies.Clear();
        __instance.currentLevel.Enemies.Add(Crack);
        __instance.currentLevel.Enemies[0].enemyType.MaxCount = 30;
        __instance.currentLevel.Enemies.Add(Coil);
        __instance.currentLevel.Enemies.Add(Bracken);
        __instance.currentLevel.Enemies.Add(GhostGirl);
        
    }

    [HarmonyPatch(typeof(RoundManager), "UnloadSceneObjectsEarly")]
    [HarmonyPostfix]
    public static void RevertEclipsed(RoundManager __instance)
    {
        if (__instance.currentLevel.currentWeather != LevelWeatherType.Eclipsed)
        {
            return;}
        
        __instance.currentLevel.OutsideEnemies  = previousLevel.OutsideEnemies;
    }
  

    [HarmonyPatch(typeof(NutcrackerEnemyAI), "GrabGun")]
    [HarmonyPrefix]

    public static bool RedoShotgun(NutcrackerEnemyAI __instance)
    {
        return true;
    }

    [HarmonyPatch(typeof(NutcrackerEnemyAI), "GrabGun")]
    [HarmonyPostfix]

    public static void SetScrapValue(NutcrackerEnemyAI __instance, GameObject gunObject)
    {
        
        
        __instance.gun = gunObject.GetComponent<ShotgunItem>();
        if ((UnityEngine.Object) __instance.gun == (UnityEngine.Object) null)
        {
            __instance.LogEnemyError("Gun in GrabGun function did not contain ShotgunItem component.");
        }
        else
        {
            Debug.Log((object) "Setting gun scrap value");
            __instance.gun.SetScrapValue(120);
            RoundManager.Instance.totalScrapValueInLevel += (float) __instance.gun.scrapValue;
            __instance.gun.parentObject = __instance.gunPoint;
            __instance.gun.isHeldByEnemy = true;
            __instance.gun.grabbableToEnemies = false;
            __instance.gun.grabbable = false;
            __instance.gun.shellsLoaded = 2;
            __instance.gun.GrabItemFromEnemy((EnemyAI) __instance);
        }
        
        
        
    }
    
  
    
    
    
    
}