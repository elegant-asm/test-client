using GameClass;
using HarmonyLib;
using Player;
using Steamworks;
using System;
using System.Diagnostics;
using System.Linq;
using TestClient.Components;
using TestClient.Modules;
using UnityEngine;
using static TestClient.Interface.ExploitPanel.Movement;

namespace TestClient.Handlers;
[HarmonyPatch]
internal class ClientPatch {
    private static readonly System.Random random = new();

    // override walkspeed
    [HarmonyPatch(typeof(Controll), "UpdateMoveSpeed")]
    [HarmonyPostfix]
    private static void UpdateMoveSpeed() {
        if (MovementModule.overrideWalkSpeedToggle.GetValue())
            Controll.speed = MovementModule.walkSpeed.GetValue();
    }
    
    [HarmonyPatch(typeof(Util), "GetBrowserPath")]
    [HarmonyPrefix]
    private static void GetBrowserPath() {
        Plugin.Log.LogMessage("[UtilAlarm] GetBrowserPath");
    }
    [HarmonyPatch(typeof(Util2), "CollectModules")]
    [HarmonyPostfix]
    private static void CollectModules(Process process, Il2CppSystem.Collections.Generic.List<Util2.Module> __result) {
        Plugin.Log.LogMessage($"[UtilAlarm] CollectModules {process.Id}, {process.ProcessName}, {process.MachineName}");
        foreach (Util2.Module module in __result)
            Plugin.Log.LogMessage($"Module: {module.ModuleName}");
    }
    [HarmonyPatch(typeof(Util3), "GetAllDerivedTypes")]
    [HarmonyPostfix]
    private static void GetAllDerivedTypes(Il2CppSystem.AppDomain aAppDomain, Il2CppSystem.Type aType, Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Il2CppSystem.Type> __result) {
        Plugin.Log.LogMessage($"[UtilAlarm] GetAllDerivedTypes ({aAppDomain.getFriendlyName()}, {aAppDomain.getDomainID()}), {aType.Name}");
        foreach (Il2CppSystem.Type type in __result)
            Plugin.Log.LogMessage($"Type: {type.Name}");
    }

    //[HarmonyPatch(typeof(Client), "recv_restorepos")]
    //[HarmonyPrefix]
    //private static bool recv_restorepos() {
    //    return !Components.TestComponent.test;
    //}

    //[HarmonyPatch(typeof(Controll), "UpdateMove2")]
    //[HarmonyPrefix]
    //private static bool UpdateMove2(Controll __instance) {
        
    //}

    //private static void WRITE_ANGLE(byte[] buffer, ref int offset, float angle) {
    //    byte angleByte = (byte)((angle / 360.0f) * 256.0f);
    //    buffer[offset] = angleByte;
    //    offset++;
    //}

    //private static void WRITE_COORD(byte[] buffer, ref int offset, float value) {
    //    ushort intValue = (ushort)(value / NET.COORDCOEF);
    //    byte[] bytes = BitConverter.GetBytes(intValue);
    //    if (!BitConverter.IsLittleEndian)
    //        Array.Reverse(bytes);
    //    Array.Copy(bytes, 0, buffer, offset, bytes.Length);
    //    offset += 2;
    //}

    //[HarmonyPatch(typeof(Controll), "UpdateSendPos")]
    //[HarmonyPrefix]
    //private static bool UpdateSendPos2() {
    //    var trControll = Controll.trControll;
    //    if (trControll == null)
    //        return false;

    //    var client = Controll.pl;
    //    if (client == null || client.bstate == 5 || client.team == 2)
    //        return false;

    //    var trCamera = Controll.trCamera;
    //    if (trCamera == null)
    //        return false;

    //    // cam euler (90 to 0, 360 to 270)
    //    float camLocalEulerAngleX = trCamera.localEulerAngles.x;

    //    // controll euler (0, 360)
    //    float controllEulerAngleY = trControll.eulerAngles.y;

    //    Vector3 currentPos = Controll.Pos;

    //    //float randomX = UnityEngine.Random.Range(-1f, 1f);
    //    //float randomY = UnityEngine.Random.Range(-1f, 1f);
    //    //float randomZ = UnityEngine.Random.Range(-1f, 1f);

    //    Vector3 newPos = Vector3.zero;

    //    Vector3 delta = newPos - currentPos;

    //    //newPos = Controll.ClampPosClip(newPos);

    //    // bitmask
    //    int mask = 0;
    //    if (delta.x > 0)
    //        mask |= (int)Controll.KeyBase.plus_x;
    //    else if (delta.x < 0)
    //        mask |= (int)Controll.KeyBase.minus_x;

    //    if (delta.z > 0)
    //        mask |= (int)Controll.KeyBase.plus_z;
    //    else if (delta.z < 0)
    //        mask |= (int)Controll.KeyBase.minus_z;

    //    Controll.movex = delta.x;
    //    Controll.movez = delta.z;

    //    //if (Controll.movex > 0)
    //    //    mask |= 1;      // Флаг движения вперёд
    //    //else if (Controll.movex < 0)
    //    //    mask |= 2;      // Флаг движения назад

    //    //if (Controll.movez > 0)
    //    //    mask |= 4;      // Флаг движения вправо (страйф)
    //    //else if (Controll.movez < 0)
    //    //    mask |= 8;      // Флаг движения влево (страйф)

    //    // Если игрок находится в воздухе, устанавливаем соответствующий флаг (например, бит 4)
    //    if (Controll.inAir)
    //        mask |= (int)Controll.KeyBase.jump;
    //    // Если игрок приседает, устанавливаем флаг (например, бит 5)
    //    if (Controll.inDuck)
    //        mask |= (int)Controll.KeyBase.duck;

    //    uint serverTime = Controll.GetServerTime();

    //    if (client.idx >= 0) {
    //        NET.BEGIN_WRITE(0xF5, 0x2D);
    //        NET.WRITE_FLOAT(newPos.x);
    //        NET.WRITE_FLOAT(newPos.y);
    //        NET.WRITE_FLOAT(newPos.z);
    //        NET.WRITE_FLOAT(camLocalEulerAngleX);
    //        NET.WRITE_FLOAT(controllEulerAngleY);
    //        NET.WRITE_BYTE((byte)mask);
    //        NET.WRITE_LONG((int)serverTime);
    //        NET.END_WRITE();

    //        Client.cs.cl_send();
    //    }
    //    return false;
    //}

    //[HarmonyPatch(typeof(Controll), "UpdateBlockAttack")]
    //[HarmonyPrefix]
    //private static bool UpdateBlockAttack() {
    //    Controll controll = Controll.cs;
    //    if (controll == null)
    //        return false;

    //    if (Controll.inStuck)
    //        return false;

    //    if (Controll.blockCursor.x < 0.0)
    //        return false;

    //    PlayerData client = Controll.pl;
    //    if (client == null)
    //        return false;

    //    WeaponData currentWeapon = client.currweapon;
    //    if (currentWeapon == null)
    //        return false;

    //    string weaponName = currentWeapon.weaponname;

    //    WeaponInv weaponInv = PLH.GetWeaponInv(client, weaponName);
    //    if (weaponInv == null)
    //        return false;

    //    if (weaponInv.backpack <= 0)
    //        return false;

    //    Controll.iattacktime = (uint)((Environment.TickCount & 0x7FFFFFFF) + 200);

    //    VWIK.shovel_attack = Time.time;

    //    Controll.inAttack = true;

    //    int x = (int)Controll.blockCursor.x;
    //    int y = (int)Controll.blockCursor.y;
    //    int z = (int)Controll.blockCursor.z;

    //    if (Controll.sblock != null) {
    //        for (int i = 0; i < Controll.sblock.Length; i++) {
    //            if (Controll.sblock == null)
    //                break;

    //            SpecialBlock specialBlock = Controll.sblock[i];
    //            if (specialBlock == null)
    //                break;

    //            if (specialBlock.inBlock(x, y, z))
    //                return false;
    //        }
    //    }

    //    weaponInv.backpack--;

    //    HUD.SetBackPack(weaponInv.backpack);

    //    MainPlayer mainPlayer = Main.Player;
    //    if (mainPlayer == null)
    //        return false;
    //    mainPlayer.SetBackPack(weaponInv.backpack);

    //    NET.BEGIN_WRITE(0xF5, 7);
    //    NET.WRITE_SHORT((short)x);
    //    NET.WRITE_SHORT((short)y);
    //    NET.WRITE_SHORT((short)z);
    //    NET.END_WRITE();
    //    Client.cs.cl_send();
    //    return false;
    //}

    private static float ConvertTo360Range(float angle) => (angle < 0f) ? angle + 360f : angle;

    // anti aim
    [HarmonyPatch(typeof(Controll), "UpdateSendPos")]
    [HarmonyPrefix]
    private static bool UpdateSendPos() {
        float camLocalEulerAngleX;
        float controllEulerAngleY;

        var trControll = Controll.trControll;
        if (trControll == null)
            return false;

        var csCam = Controll.csCam;
        if (csCam == null)
            return false;

        var client = Controll.pl;
        if (client == null || client.bstate == 5 || client.team == 2)
            return false;

        var trCamera = Controll.trCamera;
        if (trCamera == null)
            return false;

        if (AntiAimModule.antiAimToggle != null && AntiAimModule.antiAimToggle.GetValue()) {
            // cam euler (90 to 0, 360 to 270)
            camLocalEulerAngleX = ConvertTo360Range(AntiAimModule.eulerX.GetValue());
            if (AntiAimModule.oEulerX.GetValue())
                camLocalEulerAngleX += trCamera.localEulerAngles.x;

            // controll euler (0, 360)
            controllEulerAngleY = AntiAimModule.eulerY.GetValue();
            if (AntiAimModule.oEulerY.GetValue())
                controllEulerAngleY += trControll.eulerAngles.y;

            PlayerData targetData = null;
            PlayerSync targetSync = null;
            if (AntiAimModule.antiAimTarget.GetValue() == AntiAimModule.Target[1]) {
                float minPlayerDist = -1f;

                for (int i = 0; i < PLH.player.Length; i++) {
                    PlayerData data = PLH.player[i];
                    if (data == null || data.IsMainPlayer)
                        continue;
                    PlayerSync dataSync = Utility.Players.GetPlayerSyncById(data.idx);
                    if (dataSync == null || !dataSync.IsAlive)
                        continue;

                    PlayerSync clientSync = Utility.Players.GetPlayerSyncById(client.idx);
                    if (clientSync == null || !clientSync.IsAlive || Utility.Players.IsInSameTeam(clientSync, dataSync))
                        continue;

                    float clientToTargetDistance = Vector3.Distance(client.Pos, data.Pos);

                    if (targetData == null || (minPlayerDist > 0f && clientToTargetDistance < minPlayerDist) || (minPlayerDist == clientToTargetDistance)) {
                        minPlayerDist = clientToTargetDistance;
                        targetData = data;
                        targetSync = dataSync;
                        continue;
                    }
                }
            } else if (AntiAimModule.antiAimTarget.GetValue() == AntiAimModule.Target[2]) {
                float minPlayerDist = -1f;

                for (int i = 0; i < PLH.player.Length; i++) {
                    PlayerData data = PLH.player[i];
                    if (data == null || data.IsMainPlayer)
                        continue;
                    PlayerSync dataSync = Utility.Players.GetPlayerSyncById(data.idx);
                    if (dataSync == null || !dataSync.IsAlive)
                        continue;

                    PlayerSync clientSync = Utility.Players.GetPlayerSyncById(client.idx);
                    if (clientSync == null || !clientSync.IsAlive || Utility.Players.IsInSameTeam(clientSync, dataSync))
                        continue;

                    Vector3 targetPoint = csCam.WorldToScreenPoint(data.Pos);
                    if (targetPoint.z > 0f) {
                        float targetToCenterDistance = Vector2.Distance(new(Screen.width / 2, Screen.height / 2), targetPoint);

                        if (targetData == null || (minPlayerDist > 0f && targetToCenterDistance < minPlayerDist) || (minPlayerDist == targetToCenterDistance)) {
                            minPlayerDist = targetToCenterDistance;
                            targetData = data;
                            targetSync = dataSync;
                            continue;
                        }
                    }
                }
            } else if (AntiAimModule.antiAimTarget.GetValue() == AntiAimModule.Target[3] && PlayersModule.targetData != null && PlayersModule.targetSync != null) {
                if (PlayersModule.targetSync.IsAlive) {
                    targetData = PlayersModule.targetData;
                    targetSync = PlayersModule.targetSync;
                }
            }
            if (targetData != null && targetSync != null) {
                Vector3 direction = targetData.Pos - client.Pos;
                if (AntiAimModule.predict.GetValue())
                    direction += targetSync.MoveDirection;
                direction.y = 0f;
                float angleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + controllEulerAngleY;
                if (angleY < 0f)
                    angleY += 360f;
                controllEulerAngleY = angleY;
            }
        } else {
            camLocalEulerAngleX = trCamera.localEulerAngles.x;
            controllEulerAngleY = trControll.eulerAngles.y;
        }

        int mask = 0;
        if (Controll.movex > 0)
            mask |= (int)Controll.KeyBase.plus_x;
        else if (Controll.movex < 0)
            mask |= (int)Controll.KeyBase.minus_x;

        if (Controll.movez > 0)
            mask |= (int)Controll.KeyBase.plus_z;
        else if (Controll.movez < 0)
            mask |= (int)Controll.KeyBase.minus_z;

        if (Controll.inAir)
            mask |= (int)Controll.KeyBase.jump;
        if (Controll.inDuck || MovementModule.fakeDuckToggle.GetValue())
            mask |= (int)Controll.KeyBase.duck;

        //mask |= 128;

        Vector3 pos = Controll.Pos;

        uint serverTime = Controll.GetServerTime();

        if (client.idx >= 0) {
            NET.BEGIN_WRITE(0xF5, 0x2D);
            NET.WRITE_FLOAT(pos.x);
            NET.WRITE_FLOAT(pos.y);
            NET.WRITE_FLOAT(pos.z);
            NET.WRITE_FLOAT(camLocalEulerAngleX);
            NET.WRITE_FLOAT(controllEulerAngleY);
            NET.WRITE_BYTE((byte)mask);
            NET.WRITE_LONG((int)serverTime);
            NET.END_WRITE();

            Client.cs.cl_send();
        }

        if (AntisModule.noStuckStateToggle.GetValue())
            Controll.inStuck = false;

        return false;
    }

    [HarmonyPatch(typeof(Client), "recv_drop")]
    [HarmonyPrefix]
    private static void recv_drop() {
        Plugin.Log.LogWarning("recv_drop");
        //return false;
    }

    [HarmonyPatch(typeof(Client), "send_file_start")]
    [HarmonyPrefix]
    private static void send_file_start(int filetype, string filename, int filesize) {
        Plugin.Log.LogWarning($"send_file_start {filetype}, {filename}, {filesize}");
    }

    //[HarmonyPatch(typeof(PLH), "Pos")]
    //[HarmonyPrefix]
    //private static void PLH_Pos(int id, float x, float y, float z, float rx, float ry, int bitmask, int ipx, int ipy, int ipz, int irx, int iry) {
    //    //Plugin.Log.LogWarning($"Received pos {id}, mask: {bitmask}, ({bitmask & 128})");
    //    PlayerData playerData = Utility.Players.GetPlayerById(id);
    //    PlayerSync playerSync = Utility.Players.GetPlayerSyncById(id);
    //    if (playerData != null && playerSync != null && playerSync.IsClient != true) {
    //        if (id != Controll.pl.idx && (bitmask & 128) != 0)
    //            playerSync.IsClient = true;
    //    }
    //}

    //[HarmonyPatch(typeof(UIMPlay), "Awake")]
    //[HarmonyPrefix]
    //private static void UIMPlay_Awake() {
    //    Main.gameconfig = 5;
    //    MasterClient.cs.send_softpoki();
    //}

    //[HarmonyPatch(typeof(MainManager), "Awake")]
    //[HarmonyPrefix]
    //private static void MainManager_Awake() {
    //    Main.gameconfig = 5;
    //}

    //[HarmonyPatch(typeof(MainManager), "SteamInit")]
    //[HarmonyPrefix]
    //private static bool MainManager_SteamInit() {
    //    Main.gameconfig = 5;
    //    return false;
    //}

    [HarmonyPatch(typeof(Client), "send_chatmsg")]
    [HarmonyPrefix]
    private static bool send_chatmsg(int teamchat, string msg) {
        if (ChatModule.MessagesCount > 8) // smart chat
            return false;
        ChatModule.MessagesCount++;
        return true;
    }
    
    [HarmonyPatch(typeof(HUDMessage), "AddChat", [ typeof(int), typeof(string), typeof(int) ])]
    [HarmonyPrefix]
    private static void HUDMessage_AddChat(int id, ref string msg, int teamchat) {
        //Plugin.Log.LogWarning($"{id} ({teamchat}): {msg}");
        if (ChatModule.chatFilterToggle.GetValue()) {
            for (int i = 0; i < ChatModule.filterSymbols.Length; i++) {
                string filterSymbol = ChatModule.filterSymbols[i];
                msg = msg.Replace(filterSymbol, "");
            }
        }
        if (id != Controll.pl.idx && PLH.player.FirstOrDefault(player => player != null && player.idx == id) != null)
            ChatModule.MessagesCount = 0;
    }

    [HarmonyPatch(typeof(Controll), "UpdateMove2")]
    [HarmonyPrefix]
    private static bool UpdateMove2(Controll __instance) {
        if (MovementModule.betterMovementToggle != null && !MovementModule.betterMovementToggle.GetValue())
            return true;

        if (Controll.trControll == null)
            return false;

        if (Controll.csCam == null)
            return false;

        PlayerData client = Controll.pl;
        if (client == null || client.team == 2)
            return false;

        bool stepped = false;
        if (!Controll.lockMove) {
            __instance.UpdateMoveSpeed();
            __instance.UpdateMoveKey();

            if (Controll.inFreeze) {
                if (Time.time > Controll.tFreeze) {
                    Controll.inFreeze = false;
                    if (Controll.ccc != null)
                        Controll.ccc.saturation = 1.0f;
                }
                Controll.movex = 0f;
                Controll.movez = 0f;
            }

            Controll.prevPos = Controll.Pos;
            Controll.fdeltatime = Controll.fnextFrame - Controll.fcurrFrame;

            Vector3 movement = (Controll.trControll.forward * Controll.movex) + (Controll.trControll.right * Controll.movez);
            float mag = movement.magnitude;
            if (mag > 0.00001f)
                movement /= mag;
            else
                movement = Vector3.zero;

            Vector3 air = Movement.MoveAir(Vector3.zero, Controll.vel);
            Vector3 ground = Movement.MoveGround(movement, Controll.vel);
            Vector3 moved = new(ground.x, air.y, ground.z);

            float yLimit = MovementModule.targetStrafeYLimit.GetValue();
            bool ignoreSpawnProtect = MovementModule.targetStrafeIgnoreSpawnProtect.GetValue();

            if (MovementModule.targetStrafeToggle.GetValue()) {
                Vector3 targetPlayerPos = Vector3.zero;
                string strafeType = MovementModule.targetStrafeType.GetValue();
                if (strafeType == MovementModule.targetStrafeTypes[1] && PlayersModule.targetData != null && PlayersModule.targetSync != null) {
                    if (PlayersModule.targetSync.IsAlive)
                        targetPlayerPos = PlayersModule.targetData.Pos;
                } else if (strafeType == MovementModule.targetStrafeTypes[0]) {
                    float triggerDistance = MovementModule.targetStrafeDistanceTrigger.GetValue();
                    float minPlayerDist = -1f;
                    for (int i = 0; i < PLH.player.Length; i++) {
                        PlayerData data = PLH.player[i];
                        if (data == null || data.IsMainPlayer || (!ignoreSpawnProtect && data.spawnprotect))
                            continue;
                        PlayerSync dataSync = Utility.Players.GetPlayerSyncById(data.idx);
                        if (dataSync == null || !dataSync.IsAlive)
                            continue;
                        PlayerSync clientSync = Utility.Players.GetPlayerSyncById(client.idx);
                        if (clientSync == null || !clientSync.IsAlive || Utility.Players.IsInSameTeam(clientSync, dataSync))
                            continue;

                        float clientToTargetDistance = Vector3.Distance(client.Pos, data.Pos);
                        Vector3 distanceVector = client.Pos - data.Pos;

                        if (
                            clientToTargetDistance <= triggerDistance && (distanceVector.y <= yLimit &&  distanceVector.y >= -yLimit) && (targetPlayerPos == Vector3.zero ||
                            (minPlayerDist > 0f && clientToTargetDistance < minPlayerDist) || (minPlayerDist == clientToTargetDistance))
                            ) {
                            minPlayerDist = clientToTargetDistance;
                            targetPlayerPos = data.Pos;
                            continue;
                        }
                    }
                }

                if (targetPlayerPos != Vector3.zero) {
                    float spinDistance = MovementModule.targetStrafeDistance.GetValue();
                    float spinSpeed = MovementModule.targetStrafeSpeed.GetValue();
                    float spinAngle = Time.time * spinSpeed;
                    Vector3 targetPos = targetPlayerPos + new Vector3(Mathf.Cos(spinAngle) * spinDistance, 0, Mathf.Sin(spinAngle) * spinDistance);
                    Vector3 accelDir = targetPos - Controll.Pos;

                    float maxAccel = 1f;
                    if (accelDir.magnitude > maxAccel)
                        accelDir = accelDir.normalized * maxAccel;

                    Vector3 air2 = Movement.MoveAir(Vector3.zero, Controll.vel);
                    Vector3 ground2 = Movement.MoveGround(accelDir, Controll.vel);
                    moved = new Vector3(ground2.x, air2.y, ground2.z);
                }
            }

            Controll.vel = moved;

            if (Controll.vel.magnitude * Controll.speed < 0.01f)
                Controll.speed = 0f;

            if (Controll.speed > 0f) {
                Vector3 nextPos = Controll.Pos + Controll.vel * Controll.speed;

                if (VUtil.isValidBBox(nextPos, Controll.radius, Controll.boxheight)) {
                    Controll.Pos = nextPos;
                } else if (!Controll.inAir) {
                    Vector3 stepUpPos = Controll.Pos + new Vector3(0, 1f, 0);
                    Vector3 velSpeedX = new(Controll.vel.x * Controll.speed, 0f, 0f);
                    Vector3 velSpeedZ = new(0f, 0f, Controll.vel.z * Controll.speed);
                    Vector3 stepNextPos = stepUpPos + Controll.vel * Controll.speed;
                    if (VUtil.isValidBBox(stepNextPos, Controll.radius, Controll.boxheight)) {
                        stepped = true;
                        Controll.Pos = stepNextPos;
                    } else {
                        Vector3 nextPosX2 = Controll.Pos + velSpeedX;
                        if (VUtil.isValidBBox(nextPosX2, Controll.radius, Controll.boxheight)) {
                            Controll.Pos = nextPosX2;
                        } else {
                            Vector3 nextPosZ2 = Controll.Pos + velSpeedZ;
                            if (VUtil.isValidBBox(nextPosZ2, Controll.radius, Controll.boxheight)) {
                                Controll.Pos = nextPosZ2;
                            } else {
                                Vector3 stepNextPosX = stepUpPos + velSpeedX;
                                if (VUtil.isValidBBox(stepNextPosX, Controll.radius, Controll.boxheight)) {
                                    stepped = true;
                                    Controll.Pos = stepNextPosX;
                                } else {
                                    Vector3 stepNextPosZ = stepUpPos + velSpeedZ;
                                    if (VUtil.isValidBBox(stepNextPosZ, Controll.radius, Controll.boxheight)) {
                                        stepped = true;
                                        Controll.Pos = stepNextPosZ;
                                    }
                                }
                            }
                        }
                    }
                } else {
                    Vector3 nextPosX = Controll.Pos + new Vector3(Controll.vel.x * Controll.speed, Controll.vel.y * Controll.speed, 0f);
                    if (VUtil.isValidBBox(nextPosX, Controll.radius, Controll.boxheight)) {
                        Controll.Pos = nextPosX;
                    } else {
                        Vector3 nextPosZ = Controll.Pos + new Vector3(0f, Controll.vel.y * Controll.speed, Controll.vel.z * Controll.speed);
                        if (VUtil.isValidBBox(nextPosZ, Controll.radius, Controll.boxheight)) {
                            Controll.Pos = nextPosZ;
                        }
                    }
                }
            }
        }

        if (!stepped) {
            if (
                (Controll.inDuckKey && VUtil.isValidBBox(Controll.Pos + Vector3.up, Controll.radius, Controll.boxheight)) ||
                (!Controll.inDuckKey && VUtil.isValidBBox(Controll.Pos + Vector3.up, Controll.radius, Controll.boxheight))
                ) {
                __instance.UpdateMoveJump();
            } else {
                Controll.inJumpKey = true;
                Controll.inJumpKeyPressed = true;
            }
        }

        float realtime = Time.realtimeSinceStartup;
        Controll.fcurrFrame = realtime - Controll.fdifFrame;
        Controll.fnextFrame = Controll.fcurrFrame + __instance.cl_fps;
        Controll.Pos = Controll.ClampPosClip(Controll.Pos);

        return false;
    }

    //[HarmonyPatch(typeof(Client), "recv_restorepos")]
    //[HarmonyPrefix]
    //private static bool recv_restorepos(Client __instance) {
    //    //__instance.send_pos_dev(Controll.Pos.x, Controll.Pos.y, Controll.Pos.z, Controll.rx, Controll.ry, 1, Controll.GetServerTime());
    //    return false;
    //}

    //[HarmonyPatch(typeof(Client), "recv_restorepos")]
    //[HarmonyPrefix]
    //private static bool recv_restorepos() {
    //    if (!TestComponent.test)
    //        return true;
    //    Vector3 prevPos = Controll.Pos;
    //    Controll.Pos = Vector3.zero;
    //    //Controll.Pos = prevPos;
    //    return false;
    //}

    [HarmonyPatch(typeof(Controll), "UpdateMoveJump")]
    [HarmonyPrefix]
    private static bool UpdateMoveJump(Controll __instance) {
        if (
            ((Controll.inJumpKey && !Controll.inJumpKeyPressed) || MovementModule.autoJumpToggle.GetValue()) &&
            !Controll.inAir && Controll.velocity.y <= 0f
            // && VUtil.isValidBBox(Controll.Pos - Vector3.down, Controll.radius, __instance._border)
            ) {
            float jumpVelocityY = 2.35f;
            Controll.velocity = new Vector3(0.0f, jumpVelocityY, 0.0f);
            Controll.inJumpKeyPressed = true;

            Vector3 offset = Vector3.down * 0.002f;
            float timeStart = Time.time;
            VWIK.AddOffset(offset, timeStart, 0.5f);
        }
        return false;
    }

    //[HarmonyPatch(typeof(Controll), "UpdatePhysics")]
    //[HarmonyPrefix]
    private static bool Controll_UpdatePhysics() {
        if (Controll.trControll == null || Controll.freefly || Controll.pl == null || Controll.pl.team == 2)
            return false;

        Vector3 prevVelocity = Controll.velocity;
        Vector3 prevPos = Controll.Pos;

        float verticalDamping = 0f;
        if (prevVelocity.y > 0f) {
            float currYVel = prevVelocity.y;
            Controll.velocity -= Vector3.up * 0.3f;
            verticalDamping = currYVel * 0.8f;
        }

        float newY = prevPos.y + verticalDamping - 1f;
        Vector3 newPos = new(prevPos.x, newY, prevPos.z);
        if (VUtil.isValidBBox(newPos, Controll.radius, Controll.boxheight)) {
            Controll.Pos = newPos;
            Controll.inAir = true;
            Controll.inAirFrame++;
        } else {
            if (VUtil.isHeadContact()) {
                Controll.velocity = new(prevVelocity.x, 0f, prevVelocity.z);
            }
            if (VUtil.isGroundContact()) {
                if (Controll.inAir)
                    VWIK.AddOffset(Vector3.down * 0.004f, Time.time, 0.5f);
                Controll.velocity = new(prevVelocity.x, 0f, prevVelocity.z);
                Controll.inAir = false;
                Controll.inAirFrame = 0;
            }
            Controll.Pos = new(prevPos.x, Mathf.Floor(prevPos.y), prevPos.z);

            if (VoxelMap.GetBlock(Controll.Pos) != 0) {
                Vector3 posAbove = Controll.Pos + Vector3.up;
                if (VUtil.isValidBBox(posAbove, Controll.radius, Controll.boxheight))
                    Controll.Pos = posAbove;
            }
        }

        return false;
    }

    //[HarmonyPatch(typeof(Client), "send_pos_dev")]
    //[HarmonyPrefix]
    //private static void send_pos_dev(float x, float y, float z, float rx, float ry, byte bitmask, uint time) {
    //    Plugin.Log.LogWarning($"send_pos_dev ({x}, {y}, {z}), ({rx}, {ry}), {bitmask}, {time}");
    //}

    //[HarmonyPatch(typeof(Client), "cl_send")]
    //[HarmonyPrefix]
    //private static bool cl_send() {
    //    return !TestComponent.test;
    //}

    // fix to prevent server check my wi
    [HarmonyPatch(typeof(Client), "send_weaponinfo")]
    [HarmonyPrefix]
    private static bool send_weaponinfo(WeaponInfo wi) {
        return false;
    }

    //[HarmonyPatch(typeof(Client), "recv_restorepos")]
    //[HarmonyPrefix]
    //private static void recv_restorepos() {
    //    byte[] buffer = Client.buffer;
    //    int len = Client.len;
    //    NET.BEGIN_READ(buffer, len, 4);
    //    float x = NET.READ_COORD();
    //    float y = NET.READ_COORD();
    //    float z = NET.READ_COORD();

    //    Plugin.Log.LogWarning($"RestorePos (original): {x}, {y}, {z}");

    //    Vector3 newPos = new(1f, 1f, 1f);

    //    int readOffset = 4;
    //    WRITE_COORD(buffer, ref readOffset, newPos.x);
    //    WRITE_COORD(buffer, ref readOffset, newPos.y);
    //    WRITE_COORD(buffer, ref readOffset, newPos.z);

    //    NET.BEGIN_READ(buffer, len, 4);
    //    float x2 = NET.READ_COORD();
    //    float y2 = NET.READ_COORD();
    //    float z2 = NET.READ_COORD();

    //    Plugin.Log.LogWarning($"RestorePos (overriden): {x2}, {y2}, {z2}");
    //}

    [HarmonyPatch(typeof(Client), "recv_result")]
    [HarmonyPrefix]
    private static void recv_result() {
        NET.BEGIN_READ(Client.buffer, Client.len, 4);
        int _byte = NET.READ_BYTE();
        string text = NET.READ_STRING();
        Plugin.Log.LogWarning($"recv_result {_byte} = {text}");
    }
    [HarmonyPatch(typeof(Client), "send_result")]
    [HarmonyPrefix]
    private static void send_result(int c, string s) {
        Plugin.Log.LogWarning($"send_reslt {c} = {s}");
    }

    // damage multilpier
    [HarmonyPatch(typeof(Client), "send_attack")]
    [HarmonyPrefix]
    private static bool send_attack(Vector3 pos, uint time, Il2CppSystem.Collections.Generic.List<GameClass.AttackData> adlist) {
        var multiplier = WeaponsModule.damageMultiplier.GetValue();
        if (multiplier > 1 && adlist != null && adlist.Count > 0) {
            var original = adlist[0];
            for (int i = 0; i < multiplier - 1; i++)
                adlist.Add(new(original.Pointer)); // if something would go wrong, just use second constructor
        }

        NET.BEGIN_WRITE(0xF5, 4);

        if (adlist != null && adlist.Count > 0) {
            NET.WRITE_FLOAT(pos.x);
            NET.WRITE_FLOAT(pos.y);
            NET.WRITE_FLOAT(pos.z);

            NET.WRITE_LONG((int)time);

            for (int i = 0; i < adlist.Count; i++) {
                AttackData attackData = adlist[i];
                if (attackData == null) {
                    continue;
                }

                NET.WRITE_BYTE((byte)attackData.vid);
                if (WeaponsModule.alwaysRegisterAsHead.GetValue())
                    NET.WRITE_BYTE(1);
                else
                    NET.WRITE_BYTE((byte)attackData.hitbox);

                NET.WRITE_FLOAT(attackData.hitpos.x);
                NET.WRITE_FLOAT(attackData.hitpos.y);
                NET.WRITE_FLOAT(attackData.hitpos.z);
            }
        }

        NET.END_WRITE();

        Client.cs.cl_send();
        return false;
    }

    // instant grenade boom
    [HarmonyPatch(typeof(GOP), "Create")]
    [HarmonyPostfix]
    private static void GOP_Create(int c, Vector3 pos, Vector3 rot, Vector3 force, Color color, int iparam = 0, int iparam2 = 0) {
        if (AdditionalModule.instantGrenadeToggle.GetValue()) {
            for (int i = 0; i < GOP.po.Count; i++) {
                var pObj = GOP.po[i];
                if (pObj == null || pObj.dt != 5f || AdditionalModule.grenadeUsedUIDs.Contains(pObj.uid)) continue;
                AdditionalModule.grenadeUsedUIDs.Add(pObj.uid);
                Client.cs.send_entpos(pObj.uid, Vector3.zero); // zero to not boom yourself
            }
        }
    }

    // grenade tp
    // if there would be problem with entities, then it might be because of it
    [HarmonyPatch(typeof(Client), "send_entpos")]
    [HarmonyPrefix]
    private static void send_entpos(int uid, ref Vector3 pos) {
        //Plugin.Log.LogWarning($"send_entpos {uid}, {pos}");
        var killType = AdditionalModule.instantGrenadeKillType.GetValue();
        var ignoreSpawnProtect = AdditionalModule.ignoreSpawnProtectToggle.GetValue();
        if (killType != AdditionalModule.grenadeKillTypes[0]) {
            Vector3 newPos = Vector3.zero;
            PlayerSync targetSync = null;
            if (killType == AdditionalModule.grenadeKillTypes[3] && PlayersModule.targetData != null && PlayersModule.targetSync != null) {
                newPos = PlayersModule.targetData.Pos;
                targetSync = PlayersModule.targetSync;
            } else if (killType == AdditionalModule.grenadeKillTypes[2]) {
                PlayerData targetData = null;
                int maxPlayers = -1;
                var sortRange = AdditionalModule.grenadeSortRange.GetValue();
                var clientSync = Utility.Players.GetPlayerSyncById(Controll.pl.idx);
                if (clientSync == null) return;
                for (int i = 0; i < PLH.player.Length; i++) {
                    var data = PLH.player[i];
                    if (data == null || data.IsMainPlayer) continue;
                    var dataSync = Utility.Players.GetPlayerSyncById(data.idx);
                    if (dataSync == null || !Utility.Players.AllowedToDamage(clientSync, dataSync, ignoreSpawnProtect)) continue;
                    var count = 0;
                    for (int j = 0; j < PLH.player.Length; j++) {
                        var data2 = PLH.player[j];
                        if (data2 == null || data2.IsMainPlayer || data2 == data) continue;
                        var data2Sync = Utility.Players.GetPlayerSyncById(data2.idx);
                        if (data2Sync == null || !Utility.Players.AllowedToDamage(clientSync, data2Sync, ignoreSpawnProtect))
                            continue;
                        if (Vector3.Distance(data.Pos, data2.Pos) < sortRange)
                            count++;
                    }
                    if (count > maxPlayers) {
                        targetData = data;
                        targetSync = dataSync;
                        maxPlayers = count;
                    }
                }
                if (targetData != null)
                    newPos = targetData.Pos;
            } else if (killType == AdditionalModule.grenadeKillTypes[1]) {
                var clientSync = Utility.Players.GetPlayerSyncById(Controll.pl.idx);
                int attempts = 0;
                PlayerData data;
                while (true) {
                    if (attempts > 50) return;
                    attempts++;
                    data = PLH.player[random.Next(0, PLH.player.Length)];
                    if (data == null) continue;
                    var dataSync = Utility.Players.GetPlayerSyncById(data.idx);
                    if (dataSync == null || !Utility.Players.AllowedToDamage(clientSync, dataSync, ignoreSpawnProtect)) continue;
                    targetSync = dataSync;
                    break;
                }
                newPos = data.Pos;
            }
            if (newPos != Vector3.zero) {
                if (targetSync != null && targetSync.MoveDirection != Vector3.zero) {
                    newPos += targetSync.MoveDirection;
                }
                pos = newPos + Vector3.down;
            }
        }
    }

    // NoCamShake
    [HarmonyPatch(typeof(VWIK), "SetCameraShake")]
    [HarmonyPrefix]
    private static bool SetCameraShake(float amount, float speed) {
        if (AntisModule.noCameraShakeToggle.GetValue())
            return false;
        return true;
    }

    // prevents stupid exits
    [HarmonyPatch(typeof(Application), "Quit", [])]
    [HarmonyPrefix]
    private static bool Application_Quit() {
        return false;
    }

    //[HarmonyPatch(typeof(MasterClient), "ProcessData")]
    //[HarmonyPrefix]
    //private static void MasterClient_ProcessData() {
    //    //Plugin.Log.LogWarning("Buffer:");
    //    //for (int i = 0; i < MasterClient.buffer.Length; i++) {
    //    //    Plugin.Log.LogWarning($"[{i}] = {MasterClient.buffer[i]}");
    //    //}
    //    if (MasterClient.len >= 2) {
    //        //if (MasterClient.buffer[0] == 0xF5)
    //            if (MasterClient.buffer[1] == 0xA)
    //                Plugin.Log.LogWarning("ProcessData[0xA] Client TCP Drop");
    //    }
    //}

    [HarmonyPatch(typeof(PlayerPrefs), "GetFloat", [typeof(string)])]
    [HarmonyPostfix]
    private static void PlayerPrefs_GetFloat(string key, float __result) {
        Plugin.Log.LogWarning($"PlayerPrefs.GetFloat[{key}] -> {__result}");
    }
    [HarmonyPatch(typeof(PlayerPrefs), "GetInt", [typeof(string)])]
    [HarmonyPostfix]
    private static void PlayerPrefs_GetInt(string key, int __result) {
        Plugin.Log.LogWarning($"PlayerPrefs.GetInt[{key}] -> {__result}");
    }
    [HarmonyPatch(typeof(PlayerPrefs), "GetString", [typeof(string)])]
    [HarmonyPostfix]
    private static void PlayerPrefs_GetString(string key, string __result) {
        if (key.StartsWith("set_") && key.EndsWith("_uid")) return;
        Plugin.Log.LogWarning($"PlayerPrefs.GetString[{key}] -> {__result}");
    }
    [HarmonyPatch(typeof(PlayerPrefs), "SetFloat", [typeof(string), typeof(float)])]
    [HarmonyPrefix]
    private static void PlayerPrefs_SetFloat(string key, float value) {
        Plugin.Log.LogWarning($"PlayerPrefs.SetFloat[{key}] -> {value}");
    }
    [HarmonyPatch(typeof(PlayerPrefs), "SetInt", [typeof(string), typeof(int)])]
    [HarmonyPrefix]
    private static void PlayerPrefs_SetInt(string key, int value) {
        Plugin.Log.LogWarning($"PlayerPrefs.SetInt[{key}] -> {value}");
    }
    [HarmonyPatch(typeof(PlayerPrefs), "SetString", [typeof(string), typeof(string)])]
    [HarmonyPrefix]
    private static void PlayerPrefs_SetString(string key, string value) {
        Plugin.Log.LogWarning($"PlayerPrefs.SetString[{key}] -> {value}");
    }
}
