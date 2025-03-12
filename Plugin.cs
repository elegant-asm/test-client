using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TestClient.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestClient;

internal class PlayerSync {
    public PlayerData data;
    public bool IsZombie = false;
    public bool IsAlive = false;
    public bool IsClient = false;
    public Vector3 MoveDirection = Vector3.zero;
    public Dictionary<BoxCollider, Vector3> CachedColliders = [];

    public PlayerSync(PlayerData pd, GameObject[] objects) {
        data = pd;
        for (int i = 0; i < objects.Length; i++) {
            var collider = objects[i].GetComponent<BoxCollider>();
            CachedColliders.Add(collider, collider.size);
        }
    }

    public void OverrideColliders() {
        foreach (var collider in CachedColliders.Keys)
            collider.size = Vector3.zero;
    }
    public void RestoreColliders() {
        foreach (var collider in CachedColliders.Keys)
            collider.size = CachedColliders[collider];
    }
}

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[HarmonyPatch]
public class Plugin : BasePlugin {
    internal static new ManualLogSource Log;
    internal static Harmony Harmony = new("redact.TestClient");

    internal static readonly List<int> Deathmatches = [8, 2];

    internal static List<PlayerSync> Players = new(40); // 40 is default max players list

    #region Round Start/End Events
    internal static bool IsRoundStarted = false;
    internal static Utility.Events.BindableEvent OnRoundStart = new((_) => {
        IsRoundStarted = true;
        Controll.specialtime_end = 0f; // grenade glitch fix
    });
    internal static Utility.Events.BindableEvent OnRoundEnd = new((_) => {
        IsRoundStarted = false;
    });
    #endregion

    #region Player Join/Leave/Death, Players Clear Events
    /// <summary>
    /// Returns PlayerData as first arg
    /// </summary>
    internal static Utility.Events.BindableEvent OnPlayerJoin = new((_) => {

    });
    /// <summary>
    /// Returns PlayerData as first arg
    /// </summary>
    internal static Utility.Events.BindableEvent OnPlayerLeave = new();
    ///// <summary>
    ///// Returns PlayerData as first arg
    ///// </summary>
    //internal static Utility.Events.BindableEvent OnPlayerDeath = new();
    /// <summary>
    /// Returns PlayerData as first arg
    /// </summary>
    internal static Utility.Events.BindableEvent OnPlayersClear = new();
    #endregion

    //#region GameLoad Event
    //internal static bool IsGameLoaded = false;
    //internal delegate void GameLoadedEventHandler();
    //internal static event GameLoadedEventHandler OnGameLoad;
    //internal static void TriggerGameLoad() {
    //    IsGameLoaded = true;
    //    OnGameLoad?.Invoke();
    //}
    //#endregion

    internal static readonly Assembly CurrentAssembly = typeof(Plugin).Assembly;
    internal static readonly string CurrentAssemblyName = CurrentAssembly.FullName.Split(",")[0];

    public override void Load() {
        Log = base.Log;
        //OnPlayerJoin.OnEvent += (_) => { };

        SceneManager.sceneLoaded += new Action<Scene, LoadSceneMode>(OnSceneLoad);
        SceneManager.sceneUnloaded += new Action<Scene>(OnSceneUnload);

        Configuration.ConfigHandler.Init();
        World.CreateProtectedComponent<Interface.Manager>();

        Harmony.PatchAll();

        GameObject componentsHolder = World.CreateProtectedHolder();
        GameObject additionalHolder = World.CreateProtectedHolder();

        var components = CurrentAssembly.GetTypes()
            .Where(type =>
                type.Namespace == $"{CurrentAssemblyName}.Components" &&
                type.BaseType == typeof(MonoBehaviour) &&
                !type.IsNested);

        World.CreateProtectedComponent<Interface.PlayersList>(additionalHolder);
        foreach (Type component in components) {
            Log.LogWarning($"Processing component {component.Name}");
            World.CreateProtectedComponent(component, componentsHolder);
        }

        // maybe i should change players tab into separated panel?
        // try build block place, like stuck player

        // Client.send_attack
        //max attack list is 6, no matter about ammo, it's just list and you lose 1 ammo lol

        // random weapon choosed by random in GUICase.StartRoll, maybe, StartRoll have val that ig from server and it is weapon i might can change
        //GUICase.rnd
        // idk
        //GUICase.rw

        // Client.send_chatmsg(0 or 1, string)
        //0 - all chat
        //1 - team chat
        //string limit - 126

        // PlayerData.bstate
        //0 - idle
        //1 - walk
        //2 - crouch
        //3 - crouch walk
        //4 - in-air

        // default weapon
        //max ammo
        // GUIInv.GetWeaponInfo(Controll.pl.currweapon.weaponname).backpack
        //ammo
        // GUIInv.GetWeaponInfo(Controll.pl.currweapon.weaponname).ammo

        // current weapon
        //not loaded ammo
        // PLH.GetWeaponInv(client, client.currweapon.weaponname).backpack
        //ammo
        // PLH.GetWeaponInv(client, client.currweapon.weaponname).ammo

        // sets recoil to minimum, might require set to default back, because server won't let you load
        //GUIInv.GetWeaponInfo(Controll.pl.currweapon.weaponname).firerate = 0

        // players movement
        //PLH.LerpMove

        // shoot weapon
        //Controll.cs.UpdateWeaponAttack();

        // get current weapon
        //Controll.pl.GetWeaponInv(Controll.pl.currset, Controll.pl.currweapon.wi.slot)
        // get left weapon bullets
        //idk

        // server list
        //GUIPlay.srvlist
        // current server
        //GUIPlay.currServer

        // GUI onServerLoaded
        //UIMPlay.onServerLoaded
        // GUI OnCharLoaded
        //UIMPlay.OnCharLoaded

        // tbh idk what's that
        //UIMPlay.cs.sPlayers

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private static void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        Log.LogInfo($"[SCENE LOAD]: {scene.name} ({mode})");
    }
    private static void OnSceneUnload(Scene scene) {
        Log.LogInfo($"[SCENE UNLOAD]: {scene.name}");
    }
}
