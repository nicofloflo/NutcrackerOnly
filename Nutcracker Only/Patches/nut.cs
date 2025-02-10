using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;


namespace Nutcracker_Only.Patches;

public class LevelGen
{
    
    private static SpawnableEnemyWithRarity Crack = StartOfRound.Instance.levels[1].Enemies[8];
    
    private static SpawnableEnemyWithRarity Bracken  = StartOfRound.Instance.levels[5].Enemies[5];
    
    private static SpawnableEnemyWithRarity Coil = StartOfRound.Instance.levels[5].Enemies[6];
    
    private static SpawnableEnemyWithRarity GhostGirl = StartOfRound.Instance.levels[5].Enemies[0];
    
    private static EnemyType ButtCrack = new EnemyType();
    

    private static SelectableLevel NutLevel = new SelectableLevel();
    
    private static SelectableLevel previousLevel = new SelectableLevel();
    
    private static EnemyType CloneEnemyType(EnemyType original)
    {
        EnemyType copy = new EnemyType();
    
        foreach (var field in typeof(EnemyType).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
        {
            field.SetValue(copy, field.GetValue(original));
        }

        return copy;
    }

    private static SpawnableEnemyWithRarity CloneSpawnableEnemy(SpawnableEnemyWithRarity original)
    {
        SpawnableEnemyWithRarity copy = new SpawnableEnemyWithRarity();
    
        foreach (var field in typeof(SpawnableEnemyWithRarity).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
        {
            field.SetValue(copy, field.GetValue(original));
        }

        // Clone enemyType separately
        ButtCrack = CloneEnemyType(Crack.enemyType); 

        return copy;
    }

// Usage:
    private static SpawnableEnemyWithRarity OutsideNut = CloneSpawnableEnemy(Crack);
   
    
    [HarmonyPatch(typeof(RoundManager), "FinishGeneratingLevel")]
    [HarmonyPostfix]

    public static void SetNutcrackersOnly(RoundManager __instance)
    {
        OutsideNut.enemyType = ButtCrack;
        ButtCrack.isOutsideEnemy = true;
        ButtCrack.MaxCount = 70;
        ButtCrack.enemyName = "Buttcracker";
        Crack.rarity = 90;
        Bracken.rarity = 7;
        Coil.rarity = 2;
        GhostGirl.rarity = 1;
        Crack.enemyType.MaxCount = 30;
        
        NutLevel = ScriptableObject.CreateInstance<SelectableLevel>();
        NutLevel.Enemies = new List<SpawnableEnemyWithRarity>();
        NutLevel.Enemies.Add(Crack);
        NutLevel.Enemies.Add(Bracken);
        NutLevel.Enemies.Add(Coil);
        NutLevel.Enemies.Add(GhostGirl);
        NutLevel.OutsideEnemies = new List<SpawnableEnemyWithRarity>();
        NutLevel.OutsideEnemies.Add(OutsideNut);
        
        

        __instance.scrapValueMultiplier = 0;
       
        
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
            __instance.currentLevel.OutsideEnemies.Add(NutLevel.OutsideEnemies[0]);
            Debug.Log(NutLevel.OutsideEnemies[0].enemyType.isOutsideEnemy);
        }
        

        if (__instance.currentLevel.levelID == 0)
        {
            __instance.currentLevel.Enemies.Clear();
            __instance.currentLevel.Enemies.Add(NutLevel.Enemies[0]);
            __instance.minEnemiesToSpawn = 1;
            return;
            
        }
        __instance.currentLevel.Enemies.Clear();
        __instance.currentLevel.Enemies.Add(NutLevel.Enemies[0]);
        __instance.currentLevel.Enemies.Add(NutLevel.Enemies[2]);
        __instance.currentLevel.Enemies.Add(NutLevel.Enemies[1]);
        __instance.currentLevel.Enemies.Add(NutLevel.Enemies[3]);
        __instance.minEnemiesToSpawn = 2;
        
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
    
}

public class NutChanges
{
    
    //Changing Price of Shotgun
    [HarmonyPatch(typeof(NutcrackerEnemyAI), "GrabGun")]
    [HarmonyPrefix]

    public static bool RedoShotgun(NutcrackerEnemyAI __instance)
    {
        return false;
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
            __instance.gun.SetScrapValue(214);
            RoundManager.Instance.totalScrapValueInLevel += (float) __instance.gun.scrapValue;
            __instance.gun.parentObject = __instance.gunPoint;
            __instance.gun.isHeldByEnemy = true;
            __instance.gun.grabbableToEnemies = false;
            __instance.gun.grabbable = false;
            __instance.gun.shellsLoaded = 2;
            __instance.gun.GrabItemFromEnemy((EnemyAI) __instance);
        }
        
    }
    
    //Changing Shells to Eggs

    [HarmonyPatch(typeof(NutcrackerEnemyAI), "SpawnShotgunShells")]
    [HarmonyPrefix]
    public static bool RedoShells(NutcrackerEnemyAI __instance)
    {
        return false;
    }

    [HarmonyPatch(typeof(NutcrackerEnemyAI), "SpawnShotgunShells")]
    [HarmonyPostfix]
    public static void AddEggs(NutcrackerEnemyAI __instance)
    {
        if (!__instance.IsOwner)
            return;
        for (int index = 0; index < 2; ++index)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(StartOfRound.Instance.allItemsList.itemsList[69].spawnPrefab, __instance.transform.position + Vector3.up * 0.6f + new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), 0.0f, UnityEngine.Random.Range(-0.8f, 0.8f)), Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);
            gameObject.GetComponent<GrabbableObject>().fallTime = 0.0f;
            gameObject.GetComponent<NetworkObject>().Spawn();
        }
    }
}

public class EggSpawns
{
    public static SpawnableItemWithRarity Egg = StartOfRound.Instance.levels[0].spawnableScrap[22];
    public static SelectableLevel EggLevel = new SelectableLevel();
    [HarmonyPatch(typeof(RoundManager), "FinishGeneratingLevel")]
    [HarmonyPostfix]
    
    public static void SetEggsOnly(RoundManager __instance)
    {
        
        EggLevel = ScriptableObject.CreateInstance<SelectableLevel>();
        EggLevel.spawnableScrap = new List<SpawnableItemWithRarity>();
        EggLevel.spawnableScrap.Add(Egg);
        __instance.currentLevel.spawnableScrap.Clear();
        __instance.currentLevel.spawnableScrap.Add(EggLevel.spawnableScrap[0]);
        
      
    }

    [HarmonyPatch(typeof(RoundManager), "waitForScrapToSpawnToSync")]
    [HarmonyPostfix]

    public static void SetValueToZero(RoundManager __instance, NetworkObjectReference[] spawnedScrap, int[] scrapValues )
    {
        foreach (var scrap in scrapValues)
        {
            Debug.Log(scrap);
        }
    }
    
    
}