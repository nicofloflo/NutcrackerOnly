using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.ProBuilder;


namespace Nutcracker_Only.Patches;

public class LevelGen
{
    
    private static SpawnableEnemyWithRarity Crack = StartOfRound.Instance.levels[1].Enemies[8];
    
    private static SpawnableEnemyWithRarity Bracken  = StartOfRound.Instance.levels[5].Enemies[5];
    
    private static SpawnableEnemyWithRarity Coil = StartOfRound.Instance.levels[5].Enemies[6];
    
    private static SpawnableEnemyWithRarity GhostGirl = StartOfRound.Instance.levels[5].Enemies[0];

    private static SpawnableEnemyWithRarity OutsideNut = new SpawnableEnemyWithRarity();
    
    private static SelectableLevel NutLevel = new SelectableLevel();
    
    private static SelectableLevel previousLevel = new SelectableLevel();

    private static EnemyType outCracker = new EnemyType();

    [HarmonyPatch(typeof(StartOfRound), "StartGame")]
    [HarmonyPostfix]

    public static void MakeOutcracker(StartOfRound __instance)
    {
        outCracker = ScriptableObject.CreateInstance<EnemyType>();
        outCracker.enemyName = "Outcracker";
        outCracker.probabilityCurve = Crack.enemyType.probabilityCurve;
        outCracker.spawningDisabled = Crack.enemyType.spawningDisabled;
        outCracker.spawnFromWeeds = Crack.enemyType.spawnFromWeeds;
        outCracker.numberSpawnedFalloff = Crack.enemyType.numberSpawnedFalloff;
        outCracker.useNumberSpawnedFalloff = Crack.enemyType.useNumberSpawnedFalloff;
        outCracker.enemyPrefab = Crack.enemyType.enemyPrefab;
        outCracker.PowerLevel = Crack.enemyType.PowerLevel;
        outCracker.MaxCount = 70;
        outCracker.numberSpawned = Crack.enemyType.numberSpawned;
        outCracker.isOutsideEnemy = true;
        outCracker.isDaytimeEnemy = Crack.enemyType.isDaytimeEnemy;
        outCracker.increasedChanceInterior = Crack.enemyType.increasedChanceInterior;
        outCracker.normalizedTimeInDayToLeave = Crack.enemyType.normalizedTimeInDayToLeave;
        outCracker.stunTimeMultiplier = Crack.enemyType.stunTimeMultiplier;
        outCracker.doorSpeedMultiplier = Crack.enemyType.doorSpeedMultiplier;
        outCracker.stunGameDifficultyMultiplier = Crack.enemyType.stunGameDifficultyMultiplier;
        outCracker.canBeStunned = Crack.enemyType.canBeStunned;
        outCracker.canDie = Crack.enemyType.canDie;
        outCracker.canBeDestroyed = Crack.enemyType.canBeDestroyed;
        outCracker.destroyOnDeath = Crack.enemyType.destroyOnDeath;
        outCracker.canSeeThroughFog = Crack.enemyType.canSeeThroughFog;
        outCracker.pushPlayerForce = Crack.enemyType.pushPlayerForce;
        outCracker.pushPlayerDistance = Crack.enemyType.pushPlayerDistance;
        outCracker.SizeLimit = Crack.enemyType.SizeLimit;
        outCracker.timeToPlayAudio = Crack.enemyType.timeToPlayAudio;
        outCracker.loudnessMultiplier = Crack.enemyType.loudnessMultiplier;
        outCracker.overrideVentSFX = Crack.enemyType.overrideVentSFX;
        outCracker.nestSpawnPrefab = Crack.enemyType.nestSpawnPrefab;
        outCracker.nestSpawnPrefabWidth = Crack.enemyType.nestSpawnPrefabWidth;
        outCracker.useMinEnemyThresholdForNest = Crack.enemyType.useMinEnemyThresholdForNest;
        outCracker.hitBodySFX = Crack.enemyType.hitBodySFX;
        outCracker.hitEnemyVoiceSFX = Crack.enemyType.hitEnemyVoiceSFX;
        outCracker.deathSFX = Crack.enemyType.deathSFX;
        outCracker.stunSFX = Crack.enemyType.stunSFX;
        outCracker.miscAnimations = Crack.enemyType.miscAnimations;
        outCracker.audioClips = Crack.enemyType.audioClips;

        OutsideNut = new SpawnableEnemyWithRarity
        {
            enemyType = outCracker,
            rarity = Crack.rarity
        };

    }
        
    // 0 - 41 Experimentation 0.112, 4.73351, 0.448, 4.73351
    // 1 - 220 Assurance 0.392, 6.038967
    // 2 - 56 Vow 0, 1.240476, .504, 6.507092
    // 3 - 71 Gordion
    // 4 - 61 March 0.224, 4.410501
    // 5 - 20 Adamance 0f, -3f, 0.1625553f,0.006106406f, 0.4610122f, 5f, 0.7631003, 11.27556 
    // 6 - 85 Rend .056, 2.817775
    // 7 - 7 Dine 0, 2.817775, 0.336, 6.660501
    // 8 - 21 Offense 0, -3f, 0.2077699,1.22323, .504f ,7.363626f
    // 9 - 8 Titan 0,5f, 0.504, 11.27556f
    // 10 - 68 Artifice
    // 11 - 44 Liquidation
    // 12 - 5 Embrion 0f,1.516304f, .448,7.363626
    //
    // 0 = 6 AM
    // 0.056 = 7 AM
    // 0.112 = 8 AM
    // 0.168 = 9 AM
    // 0.224 = 10 AM
    // 0.28 = 11 AM
    // 0.336 = 12 AM
    // 0.392 = 1 PM
    // 0.448 = 2 PM
    // 0.504 = 3 PM
    // 0.56 = 4 PM
    // 0.616 = 5 PM
    // 0.672 = 6 PM
    // 0.728 = 7 PM
    // 0.784 = 8 PM
    // 0.84 = 9 PM
    // 0.896 = 10 PM
    // 0.952 = 11 PM
    // 1 = 12 AM

    [HarmonyPatch(typeof(StartOfRound), "StartGame")]
    [HarmonyPostfix]
    public static void CreateUpdatedAnimationCurve(StartOfRound __instance)
    {
        foreach (var level in __instance.levels)
        {
            Debug.Log(level.PlanetName);
        }
        
        __instance.levels[0].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f, -3f), new Keyframe(0.195999f, 4.73351f), new Keyframe(0.7593884f,4.973001f), new Keyframe(1f,15f));
        __instance.levels[1].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f, -3f), new Keyframe(0.4999272f, 6.038967f), new Keyframe(1f, 15f));
        __instance.levels[2].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f , 1.240476f), new Keyframe(.504f, 6.507092f), new Keyframe(1f, 15f));
        __instance.levels[4].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f, -3f), new Keyframe(0.224f,4.410501f), new Keyframe(1f, 15f));
        __instance.levels[4].outsideEnemySpawnChanceThroughDay = new AnimationCurve(new Keyframe(0f,1f), new Keyframe(0.28f,2f), new Keyframe(0.8769338f, 3.964059f), new Keyframe(1f, 5.359401f));
        __instance.levels[5].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f,-3f), new Keyframe(0.1625553f,0.006106406f), new Keyframe(0.504f, 5f), new Keyframe(0.7631003f, 11.27556f), new Keyframe(1f, 15f));
        __instance.levels[6].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0, -3f), new Keyframe(.056f, 2.817775f), new Keyframe(1f, 15f));
        __instance.levels[7].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f, 2.817775f), new Keyframe(0.336f, 6.660501f), new Keyframe(1f, 15f));
        __instance.levels[8].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0, -3f), new Keyframe(0.2077699f,1.22323f), new Keyframe(.504f ,7.363626f),new Keyframe(1f, 15f));
        __instance.levels[9].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f,5f), new Keyframe(0.504f, 11.27556f), new Keyframe(1f, 15f));
        __instance.levels[10].dungeonFlowTypes = __instance.levels[7].dungeonFlowTypes;
        __instance.levels[10].maxEnemyPowerCount = 24;
        __instance.levels[10].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f, 7f), new Keyframe(0.448f,12f), new Keyframe(1f, 15f));
        __instance.levels[12].enemySpawnChanceThroughoutDay = new AnimationCurve(new Keyframe(0f,1.516304f), new Keyframe(.448f,7.363626f),new Keyframe(1f, 15f));

    }
    
    [HarmonyPatch(typeof(RoundManager), "FinishGeneratingLevel")]
    [HarmonyPostfix]
    public static void SetNutcrackersOnly(RoundManager __instance)
    {
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
            
        }
        

        if (__instance.currentLevel.levelID == 0)
        {
            __instance.currentLevel.Enemies.Clear();
            __instance.currentLevel.Enemies.Add(NutLevel.Enemies[0]);
            __instance.minEnemiesToSpawn = 1;
            return;
            
        }

        if (__instance.currentLevel.levelID == 4)
        {
            __instance.currentLevel.OutsideEnemies.Clear();
            __instance.currentLevel.OutsideEnemies.Add(NutLevel.OutsideEnemies[0]);
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
            __instance.gun.SetScrapValue(60);
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

    [HarmonyPatch(typeof(NutcrackerEnemyAI), "Update")]
    [HarmonyPostfix]

    public static void CheckIfOutside(NutcrackerEnemyAI __instance)
    {

        if ((double)__instance.transform.position.y > -80.0)
        {
            __instance.SetEnemyOutside(true);
        }
        
        
        
    }
}

public class EggSpawns
{
    private static SpawnableItemWithRarity Egg = StartOfRound.Instance.levels[0].spawnableScrap[22];
    private static SelectableLevel EggLevel = new SelectableLevel();
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

public class ExpFix
{
    [HarmonyPatch(typeof(HUDManager), "SetPlayerLevel")]
    [HarmonyPrefix]
    
    public static void SetPlayerLevel(HUDManager __instance)
    {
        RoundManager.Instance.totalScrapValueInLevel += 3;
    }
}

public class InterestEggs
{
    [HarmonyPatch(typeof(StartOfRound), "GetValueOfAllScrap")]
    [HarmonyPrefix]
    public static bool EggInterest(ref int __result, StartOfRound __instance, bool onlyScrapCollected = true, bool onlyNewScrap = false)
    {
        GrabbableObject[] objectsOfType = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();
        int valueOfAllScrap = 0;
        for (int index = 0; index < objectsOfType.Length; ++index)
        {
            if (objectsOfType[index].itemProperties.itemName == "Easter egg")
            {
                objectsOfType[index].SetScrapValue(objectsOfType[index].scrapValue + 3);
            }

            if (__instance.shipInnerRoomBounds.bounds.Contains(objectsOfType[index].transform.position))
                objectsOfType[index].isInShipRoom = true;
        }

        for (int index = 0; index < objectsOfType.Length; ++index)
        {
            if ((!onlyNewScrap || !objectsOfType[index].scrapPersistedThroughRounds) &&
                objectsOfType[index].itemProperties.isScrap && !objectsOfType[index].deactivated &&
                !objectsOfType[index].itemUsedUp && (objectsOfType[index].isInShipRoom || !onlyScrapCollected))
                valueOfAllScrap += objectsOfType[index].scrapValue;
        }

        __result = valueOfAllScrap;
        
        return false;
    }
}