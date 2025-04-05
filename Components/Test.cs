using BepInEx.Unity.IL2CPP.Utils.Collections;
using Player;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UniverseLib;

namespace TestClient.Components;

internal class TestComponent : MonoBehaviour {
    //void Start() {
    //    StartCoroutine(test().WrapToIl2Cpp());
    //}

    //private static System.Random random = new();
    //private static IEnumerator test() {
    //    while (true) {
    //        if (Controll.cs != null && Controll.pl != null) {
    //            Controll.pl.bstate = 4;
    //            Controll.pl.prevbstate = 4;
    //            NET.BEGIN_WRITE();
    //            NET.WRITE_FLOAT(Time.time);
    //            NET.WRITE_SHORT(1);
    //            NET.WRITE_SHORT(1);
    //            NET.WRITE_SHORT(1);
    //            Client.cs.recv_restorepos();
    //        }
    //        yield return null;
    //    }
    //}

    private static readonly string[] IPs = ["85.119.149.187"];
    // public servers ips: 85.119.149.35, 82.202.247.45
    // user servers ips: 85.119.149.187
    // prob better would be get server info somehow
    private static IEnumerator ConnectThread() {
        foreach (var ip in IPs) {
            for (var i = 40500; i < 40600; i++) {
                Client.PORT = i;
                Client.IP = ip;
                Client.cs.Connect();

                yield return new WaitForSeconds(0.5f);

                if (Client.isConnected()) {
                    Plugin.Log.LogWarning($"Connected to {ip}:{i}");
                    if (PLH.player.Where(player => player != null).Count() < 2) {
                        Plugin.Log.LogWarning("Created new server, disconnecting.");
                        //Client.cs.Disconnect();
                        GUIGameExit.ExitMainMenu();
                    } else
                        break;
                }

                //yield return new WaitForSeconds(1.5f);
            }
        }
    }

    private static IEnumerator ThrowGrenadeAndReconnect() {
        Controll.UseSpecial();
        yield return new WaitForSeconds(0.1f);
        GUIGameExit.ExitMainMenu();
        Client.cs.Connect();
    }

    //private void Connect(string ip = null, int port = 0) {
    //    Client client = Client.cs;

    //    client.Active = false;

    //    if (client.client != null && client.client.Connected) {
    //        NetworkStream stream = client.client.GetStream();
    //        if (stream != null) {
    //            stream.Close();
    //        }
    //        client.client.Close();
    //        client = null;
    //    }

    //    client.client = new TcpClient();
    //    if (client == null) {
    //        Plugin.Log.LogWarning("[CUSTOM_CONNECT] Failed to create TcpClient!");
    //        return;
    //    }

    //    client.client.NoDelay = true;

    //    Il2CppSystem.IAsyncResult asyncResult = client.client.BeginConnect(ip ?? Client.IP, port != 0 ? port : Client.PORT, null, null);
    //    if (asyncResult == null) {
    //        Plugin.Log.LogWarning("[CUSTOM_CONNECT] Connection failed! (BeginConnect)");
    //        return;
    //    }

    //    bool connected = asyncResult.AsyncWaitHandle.WaitOne(2000);
    //    if (!connected || client == null) {
    //        Plugin.Log.LogWarning("[CUSTOM_CONNECT] Connection failed! (After Wait)");
    //        return;
    //    }

    //    if (client.client.Connected) {
    //        Plugin.Log.LogWarning("[CUSTOM_CONNECT] Connected!");
    //        client.client.EndConnect(asyncResult);
    //        NetworkStream stream = client.client.GetStream();
    //        if (stream == null || client.readBuffer == null)
    //            return;

    //        stream.BeginRead(client.readBuffer, 0, client.readBuffer.Length, new Il2CppSystem.AsyncCallback(client.GetActualType().GetMethod("DoRead").GetLdftnPointer()), this);

    //        client.Active = true;
    //    } else {
    //        Plugin.Log.LogWarning("[CUSTOM_CONNECT] Not connected!");
    //        client.Active = false;
    //    }

    //    if (!client.Active) {
    //        Plugin.Log.LogWarning("[CUSTOM_CONNECT] Connection failed!");
    //        return;
    //    }

    //    //DemoRec.StartRecord();
    //    //Main.HideMenus();
    //    client.send_auth();

    //    Plugin.Log.LogWarning("[CUSTOM_CONNECT] Connection success!");
    //}

    private static IEnumerator TestLoop() {
        RaycastHit hit;
        if (Physics.Raycast(Controll.csCam.transform.position, Controll.csCam.transform.forward, out hit, float.MaxValue)) {
            Vector3 startPos = Controll.Pos;
            Vector3 endPos = hit.point;
            float distance = Vector3.Distance(startPos, endPos);
            Vector3 direction = (endPos - startPos).normalized;

            for (float d = 0; d < distance; d += 0.25f) {
                Controll.Pos = startPos + direction * d;
                yield return new WaitForSeconds(0.1f);
            }

            Controll.Pos = endPos;
        }
    }

    public static bool test = false;
    public static bool test2 = false;
    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            //Client.cs.send_spectator();
            //MasterClient.cs.send_auth_gamelocal();
            GUIGameExit.ExitMainMenu();
            Client.cs.Connect();

            //test = !test;
            //Plugin.Log.LogWarning($"test {test}");
            //EngineSettings.ApplyEngineMode(EngineMode.Battleroyale);
            //EngineSettings.ControlSnapToBlock = true;
            //EngineSettings.NewUI = true;
            //Plugin.Log.LogWarning($"a {Controll.pl.bstate}, {Controll.pl.prevbstate}, {Controll.pl.wstate}");
            //Plugin.Log.LogWarning($"a {Controll.vel}");
            //Client.cs.send_pos_dev(Controll.Pos.x - 5.5f, Controll.Pos.y, Controll.Pos.z - 5.5f, Controll.rx, Controll.ry, 4, Controll.GetServerTime());
        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            //Vector3 blockPos = Controll.Pos;
            //VoxelMap.SetBlock((int)blockPos.x, (int)blockPos.y, (int)blockPos.z, Color.magenta, 1);
            //VoxelMap.RenderBlock((int)blockPos.x, (int)blockPos.y, (int)blockPos.z);
            //    test = true;
            //Controll.Pos = new(Controll.Pos.x, -1f, Controll.Pos.z);
            //    Plugin.Log.LogWarning(test);

            //    //StartCoroutine(TestLoop().WrapToIl2Cpp());

            //    //int mask = 0;
            //    //mask |= (int)Controll.KeyBase.minus_x;
            //    //mask |= (int)Controll.KeyBase.minus_z;
            //    //mask |= (int)Controll.KeyBase.jump;
        }
        //if (Input.GetKeyDown(KeyCode.F6)) {
        //    test2 = !test2;
        //    Plugin.Log.LogWarning(test2);

        //    //int mask = 0;
        //    //mask |= (int)Controll.KeyBase.minus_x;
        //    //mask |= (int)Controll.KeyBase.minus_z;
        //    //mask |= (int)Controll.KeyBase.jump;
        //    //Client.cs.send_pos_dev(Controll.Pos.x-5.5f, Controll.Pos.y, Controll.Pos.z-5.5f, Controll.rx, Controll.ry, 4, Controll.GetServerTime());
        //}
        if (Input.GetKeyDown(KeyCode.F3)) {
            StartCoroutine(ConnectThread().WrapToIl2Cpp());
        }
        if (Input.GetKeyDown(KeyCode.F4)) {
            StartCoroutine(ThrowGrenadeAndReconnect().WrapToIl2Cpp());
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            //Plugin.Log.LogWarning($"ds {Controll.default_speed}");
            if (Controll.pl != null && Controll.pl.currweapon != null) {
                WeaponInfo wInfo = GUIInv.GetWeaponInfo(Controll.pl.currweapon.weaponname);
                Plugin.Log.LogWarning(Controll.nextweaponkey);
                Plugin.Log.LogWarning($"[{wInfo.slot}] ({wInfo.name}, {wInfo.id}), {wInfo.recoil}, {wInfo.accuracy}, {wInfo.firerate}, {wInfo.firetype}, {wInfo.piercing}, {Controll.pl.currweapon.wi.firetype}");
            }
            Plugin.Log.LogWarning(Controll.pl.bstate);
            Plugin.Log.LogWarning(Client.isConnected());

            //Plugin.Log.LogWarning("MaxAirAccel " + Movement.max_velocity_air);
            //Plugin.Log.LogWarning("MaxGroundAccel " + Movement.max_velocity_ground);
            //Plugin.Log.LogWarning("AirAccel " + Movement.air_accelerate);
            //Plugin.Log.LogWarning("GroundAccel " + Movement.ground_accelerate);

            //AssetBundle bundle = Utility.Bundles.LoadBundle("test");
            //AudioSource audioSource = bundle.LoadAsset<AudioSource>("CashRegisterEffect");
            //audioSource.Play();

            //Controll.specialtime_end = 0f;

            //Client.cs.send_weaponselect(1000);
            //PLH.ForceSelectWeapon(Controll.pl.idx, 1000);
            //PLH.SelectWeapon(Controll.pl, "zombiehands");

            //var serversList = GUIPlay.srvlist;
            //if (serversList != null )
            //    for (int i = 0; i < serversList.Count; i++) {
            //        var server = serversList[i];
            //        Plugin.Log.LogWarning($"ID: {server.idx + 1}, IP: {server.ip}, PORT: {server.port}, STATUS: {server.status}, C: ({server.players}/{server.maxplayers}), ISPRIVATE: {server.privategame}");
            //    }

            //for (int i = 0; i < 5000; i++) {
            //    Client.cs.send_entpos(i, Vector3.zero);
            //}

            //if (Controll.pl != null) {
            //    Plugin.Log.LogWarning($"p to g {Vector3.Distance(Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name.StartsWith("grenade_")).transform.position, Controll.currPos)} units");
            //}

            //Controll.cs.UpdateBlockAttack();
            //Client.cs.send_attackblock((int)Controll.pl.currPos.x, (int)Controll.pl.currPos.y, (int)Controll.pl.currPos.z);
            //Controll.pl.currweapon.wi.firetype = 0;

            //for (int i = 0; i < PLH.player.Length; i++) {
            //    var data = PLH.player[i];
            //    if (data == null || data.IsMainPlayer) continue;
            //    var dataSync = Utility.Players.GetPlayerSyncById(data.idx);
            //    if (dataSync == null) continue;
            //    Plugin.Log.LogWarning($"player mov dir {dataSync.MoveDirection}");
            //    //var p1 = data.Pos;
            //    //var p2 = data.currPos;
            //    //Plugin.Log.LogWarning($"pos {p1}");
            //    //Plugin.Log.LogWarning($"currpos {p2}");
            //}
            //MasterClient.cs.send_questreward(7);
            //if (GUICase.itemcase != null) {
            //    Plugin.Log.LogWarning(GUICase.itemcase.ci?.name);
            //    Plugin.Log.LogWarning(GUICase.itemcase.ci?.idx);
            //    foreach (var wInfo in GUICase.itemcase.ci?.weapon)
            //        Plugin.Log.LogWarning(wInfo.name);
            //    MasterClient.cs.send_caseopen(GUICase.itemcase.ci.idx, 8);
            //}
            //StartCoroutine(sendArrayMsg(nazi).WrapToIl2Cpp());
            //Utility.Weapon.Get(Controll.pl.currweapon.weaponname).accuracy = 0;
            //Utility.Weapon.Get(Controll.pl.currweapon.weaponname).recoil = 0;
            //Plugin.Log.LogWarning("attack");
            //for (int i = 0; i < PLH.player.Length; i++) {
            //    var data = PLH.player[i];
            //    if (data == null || data.IsMainPlayer)
            //        continue;
            //    Plugin.Log.LogWarning("attack2");
            //    //Client.cs.send_attackthrow(data.currPos, new(), 0);
            //    //Client.cs.send_receivethrow(Controll.pl.idx, data.currPos, data.idx);
            //    //Client.cs.send_damagethrow(data.idx);
            //}
        }
        //Builder.UpdateFlyMode();
        //if (Input.GetKeyDown(KeyCode.F3)) {
        //    if (Controll.pl != null) {
        //        for (int i = 0; i < PLH.player.Length; i++) {
        //            var data = PLH.player[i];
        //            if (data == null || data.IsMainPlayer)
        //                continue;
        //            var head = data.goLLeg[2];
        //            var hitData = head.GetComponent<HitData>();

        //            Il2CppSystem.Collections.Generic.List<GameClass.AttackData> attackList = new();
        //            for (int j = 0; j < 7; j++)
        //                attackList.Add(new GameClass.AttackData(data.idx, hitData.box, head.transform.position));
        //            Client.cs.send_attack(Controll.csCam.transform.position, (uint)Time.time, attackList);
        //            break;
        //        }
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.F4)) {
        //    if (Controll.pl != null) {
        //        for (int i = 0; i < PLH.player.Length; i++) {
        //            var data = PLH.player[i];
        //            if (data == null || data.IsMainPlayer)
        //                continue;
        //            var head = data.goHead;
        //            var hitData = head.GetComponent<HitData>();

        //            Il2CppSystem.Collections.Generic.List<GameClass.AttackData> attackList = new();
        //            attackList.Add(new GameClass.AttackData(data.idx, hitData.box, head.transform.position));
        //            Client.cs.send_attack(Controll.csCam.transform.position, (uint)Time.time, attackList);
        //            break;
        //        }
        //    }
        //}
    }
}