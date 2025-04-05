using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestClient.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestClient;

internal class PlayerSync {
    public PlayerData data;
    public bool IsZombie = false;
    public bool IsAlive = false;
    //public bool IsClient = false;
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
public class Plugin : BasePlugin {
    internal static new ManualLogSource Log;
    internal static Harmony Harmony = new("elegant-asm.TestClient");

    internal static readonly List<int> Deathmatches = [8, 2];

    internal static List<PlayerSync> Players = new(40); // 40 is default max players list

    #region Round Start/End Events
    internal static bool IsRoundStarted = false;
    internal static bool IsGameInProgress = false;
    internal static Utility.Events.BindableEvent OnRoundStart = new((_) => {
        IsRoundStarted = true;
        IsGameInProgress = true;
        Controll.specialtime_end = 0f; // grenade glitch fix
    });
    internal static Utility.Events.BindableEvent OnRoundEnd = new((_) => {
        IsRoundStarted = false;
    });
    #endregion

    #region Player Join/Leave/Spawn/Death, Players Clear Events
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
    internal static Utility.Events.BindableEvent OnPlayerSpawn = new();
    ///// <summary>
    ///// Returns PlayerData as first arg
    ///// </summary>
    //internal static Utility.Events.BindableEvent OnPlayerDeath = new();
    /// <summary>
    /// Returns PlayerData as first arg
    /// </summary>
    internal static Utility.Events.BindableEvent OnPlayersClear = new((_) => {
        IsGameInProgress = false;
    });
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

        // TODO: CHECK ALL PLAYERPREFS USAGES

        // Client.ProcessData
        // proto 0xF5

        // MasterClient.ProcessData
        // MasterClient.buffer[0] - proto
        // MasterClient.buffer[1] - packetid
        // if packet > 0xC8
        //  if packet > 0xDD
        //   0xE6 - vk auth
        //   0xE7 - isvk + show gold gui
        //   0xF0 - 
        //  0xCA - removed some key from PlayerPrefs
        //  0xDD - sends something back to server (requires debug)
        //  sends vk auth
        // if packet <= 0x5A
        //  0 - sets authed to true
        //  1 - does something with rRightMsg and sOnline, if vk does something additional
        //  2 - sets ip and port
        //  3 - recv_list
        //  5 - recv_customplay
        //  6 - recv_acq
        //  0xA - closes TCP (never got it)
        //  0x1E - updates weapon frags
        //  0x29 - recv_set
        //  0x2B - recv_stats
        //  0x2C - recv_options
        //  0x2E - recv_character
        //  0x30 - recv_name
        //  0x31 - recv_profile
        //  0x32 - recv_inv_(weapon/case/key/playerskin)_pass
        //  0x33 - recv_weaponinfo
        //  0x35 - recv_caseinfo
        //  0x36 - recv_keyinfo
        //  0x37 - interacts with GUIShop, sets buycode and if buycode = 1 then inlock = 0
        //  0x38 - starts case roll
        //  0x39 - ends craft
        //  0x3A - recv_questlist
        //  0x3B - Main.SetMenuNewItem
        //  0x3C - recv_topplayer
        //  0x3E - recv_questload
        //  0x40 - recv_shop
        //  0x41 - GUICraft sell end
        //  0x42 - sets GUICraft cs_sec to received long value
        //  0x43 - GUIBonus set
        //  0x44 - recv_clanbase
        //  0x45 - sets clanid owner and clanname owner to received values
        //  0x46 - recv_clancreate
        //  0x47 - recv_clanlist
        //  0x48 - recv_clanfind
        //  0x49 - recv_clanplayer
        //  0x4B - recv_clanmessage
        //  0x50 - sets clanstatus and clannamecount
        //  0x52 - recv_clanstats
        //  0x53 - recv_clanplayerstats
        //  0x54 - recv_clanchatlist
        //  0x55 - recv_clanchatsend
        //  0x56 - recv_topclan
        //  0x57 - sets something to received value
        //  0x58 - recv_playerskininfo
        //  0x5A - sets mainmanager mod to true, mkey to received string value
        //  4, 7, 8, 9 empty
        // 0x6F - console log message
        // 0x70 - recv_error
        // 0xC8 - if steam sends vk auth

        // Client - server connection controller
        // MasterClient - user connection controller

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
