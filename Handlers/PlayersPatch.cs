using HarmonyLib;
using Player;
using TestClient.Modules;

namespace TestClient.Handlers;

[HarmonyPatch]
internal class PlayersPatch {
    [HarmonyPatch(typeof(PLH), "Add")]
    [HarmonyPostfix]
    private static void PLH_Add(int id, string name, int team, int cp0, int cp1, int cp2, int cp3, int cp4, int cp5, int cp6, int cp7, int cp8, int cp9, int pexp, int cid, string cname) {
        //Plugin.Log.LogWarning($"Player added: {id}, {name}");
        //Plugin.Log.LogWarning($"{team}, {cp0}, {cp1}, {cp2}, {cp3}, {cp4}, {cp5}, {cp6}, {cp7}, {cp8}, {cp9}, {pexp}, {cid}, {cname}");
        PlayerData player = Utility.Players.GetPlayerById(id);
        if (player != null) {
            Plugin.Players.Add(new(player, [
                    player.goHead, player.goBody,
                    player.goRArm[0], player.goRArm[1], player.goLArm[0], player.goLArm[1],
                    player.goRLeg[0], player.goRLeg[1], player.goRLeg[2], player.goLLeg[0], player.goLLeg[1], player.goLLeg[2]
                ]));
            Plugin.OnPlayerJoin.Trigger([player]);
        }
    }

    [HarmonyPatch(typeof(PLH), "Delete")]
    [HarmonyPrefix]
    private static void PLH_Delete(int id) {
        //Plugin.Log.LogWarning($"Player deleted: {id}");
        PlayerData data = Utility.Players.GetPlayerById(id);
        if (data != null) {
            Plugin.Players.Remove(Utility.Players.GetPlayerSyncById(id));
            Plugin.OnPlayerLeave.Trigger([data]);
        }
    }

    [HarmonyPatch(typeof(PLH), "Clear")]
    [HarmonyPrefix]
    private static void PLH_Clear() {
        //Plugin.Log.LogWarning($"Players cleared.");
        Plugin.OnPlayersClear.Trigger(); // not sure if it supposed to be here, not after clearing
        if (Plugin.IsRoundStarted)
            Plugin.OnRoundEnd.Trigger();
        Plugin.Players.Clear();
    }

    //[HarmonyPatch(typeof(PLH), "UpdateTeam")]
    //[HarmonyPrefix]
    //private static void PLH_UpdateTeam(int id, int team) {
    //    Plugin.Log.LogWarning($"UpdateTeam for {id} to {team}");
    //}

    [HarmonyPatch(typeof(PLH), "Spawn")]
    [HarmonyPrefix]
    private static void PLH_Spawn(int id, int x, int y, int z, float ry, int weaponset) {
        //Plugin.Log.LogWarning($"Player spawn: {id}, ({x}, {y}, {z}), {ry}, {weaponset}");
        if (!Plugin.IsRoundStarted)
            Plugin.OnRoundStart.Trigger();

        PlayerData data = Utility.Players.GetPlayerById(id);
        if (data != null) {
            Plugin.OnPlayerSpawn.Trigger([data]);
            PlayerSync playerSync = Utility.Players.GetPlayerSyncById(id);
            if (playerSync != null) {
                playerSync.IsAlive = true;
                playerSync.IsZombie = false;

                // no colliders for teammates
                CheckColliders(id, playerSync);
            }
        }
    }

    //[HarmonyPatch(typeof(PLH), "RefillWeapons")]
    //[HarmonyPrefix]
    //private static void PLH_RefillWeapons() {
    //    Plugin.Log.LogWarning("RefillWeapons");
    //}

    [HarmonyPatch(typeof(PLH), "SetNormal")]
    [HarmonyPrefix]
    private static void PLH_SetNormal(int id) {
        Plugin.Log.LogWarning($"SetNormal: {id}");
    }

    [HarmonyPatch(typeof(PLH), "SetZombie")]
    [HarmonyPrefix]
    private static void PLH_SetZombie(int id) {
        //Plugin.Log.LogWarning($"SetZombie: {id}");
        PlayerData data = Utility.Players.GetPlayerById(id);
        if (data != null)
            Utility.Players.GetPlayerSyncById(id).IsZombie = true;

        PlayerSync playerSync = Utility.Players.GetPlayerSyncById(id);
        if (playerSync != null)
            CheckColliders(id, playerSync);
    }

    private static void CheckColliders(int id, PlayerSync playerSync) {
        if (id == Controll.pl.idx) {
            for (int i = 0; i < PLH.player.Count; i++) {
                PlayerData targetData = PLH.player[i];
                if (targetData == null || targetData.IsMainPlayer)
                    continue;
                PlayerSync targetSync = Utility.Players.GetPlayerSyncById(targetData.idx);
                if (targetSync == null)
                    continue;
                if (Utility.Players.IsInSameTeam(playerSync, targetSync))
                    targetSync.OverrideColliders();
                else
                    targetSync.RestoreColliders();
            }
        } else {
            PlayerData client = Utility.Players.GetPlayerById(Controll.pl.idx);
            PlayerSync clientSync = Utility.Players.GetPlayerSyncById(client.idx);

            if (Utility.Players.IsInSameTeam(clientSync, playerSync))
                playerSync.OverrideColliders();
            else
                playerSync.RestoreColliders();
        }
    }

    //[HarmonyPatch(typeof(PLH), "SetDamaged")]
    //[HarmonyPrefix]
    //private static void PLH_SetDamaged(int id) {
    //    Plugin.Log.LogWarning($"SetDamaged: {id}");
    //}

    //[HarmonyPatch(typeof(PLH), "SetColor")]
    //[HarmonyPrefix]
    //private static void PLH_SetColor(int id, Color c) {
    //    Plugin.Log.LogWarning($"SetColor: {id}, ({c.r}, {c.g}, {c.b}, {c.a})");
    //}

    //[HarmonyPatch(typeof(PLH), "Pos")]
    //[HarmonyPrefix]
    //private static void PLH_Pos(int id, float x, float y, float z, float rx, float ry, int bitmask, int ipx, int ipy, int ipz, int irx, int iry) {
    //    Plugin.Log.LogWarning($"Pos: {id}, ({x}, {y}, {z}), ({rx}, {ry}), {bitmask}, ({ipx}, {ipy}, {ipz}), ({irx}, {iry})");
    //}

    //[HarmonyPatch(typeof(PLH), "Attack")]
    //[HarmonyPrefix]
    //private static void PLH_Attack(int id) {
    //    Plugin.Log.LogWarning($"Attack: {id}");
    //}

    //[HarmonyPatch(typeof(PLH), "AttackDamage")]
    //[HarmonyPrefix]
    //private static void PLH_AttackDamage(int id, int vid, int vbox) {
    //    Plugin.Log.LogWarning($"AttackDamage: {id}, {vid}, {vbox}");
    //}

    //[HarmonyPatch(typeof(PLH), "AttackDamageRepeat")]
    //[HarmonyPrefix]
    //private static void PLH_AttackDamageRepeat(int id, int vid, int vbox, int bloodid, int soundid, int affectid) {
    //    Plugin.Log.LogWarning($"AttackDamageRepeat: {id}, {vid}, {vbox}, {bloodid}, {soundid}, {affectid}");
    //}

    [HarmonyPatch(typeof(PLH), "Kill")]
    [HarmonyPrefix]
    private static void PLH_Kill(int id, int aid, int hitbox) {
        //Plugin.Log.LogWarning($"Kill: {id}, {aid}, {hitbox}");
        //Plugin.OnPlayerDeath.Trigger();
        PlayerData data = Utility.Players.GetPlayerById(id);
        if (data == null) return;
        PlayerSync dataSync = Utility.Players.GetPlayerSyncById(id);
        if (dataSync == null) return;
        dataSync.IsAlive = false;

        PlayerSync clientSync = Utility.Players.GetPlayerSyncById(Controll.pl.idx);

        if (id != Controll.pl.idx && aid == Controll.pl.idx && !Utility.Players.IsInSameTeam(clientSync, dataSync) && ChatModule.sendOnKillToggle.GetValue()) {
            string value = ChatModule.messages[ChatModule.chatMessage.GetValue()].Value;
            if (value != null)
                ChatModule.SendMessage(ChatModule.messages[ChatModule.chatMessage.GetValue()].Value);
        }
    }

    //[HarmonyPatch(typeof(PLH), "AddWeapon")]
    //[HarmonyPrefix]
    //private static void PLH_AddWeapon(PlayerData p, WeaponData wd) {
    //    Plugin.Log.LogWarning($"AddWeapon: {p.idx}, {wd.weaponname}");
    //}

    //[HarmonyPatch(typeof(PLH), "HideWeapon")]
    //[HarmonyPrefix]
    //private static void PLH_HideWeapon(PlayerData p) {
    //    Plugin.Log.LogWarning($"HideWeapon: {p.idx}");
    //}

    //[HarmonyPatch(typeof(PLH), "ClearWeapon")]
    //[HarmonyPrefix]
    //private static void PLH_ClearWeapon(PlayerData p) {
    //    Plugin.Log.LogWarning($"ClearWeapon: {p.idx}");
    //}

    [HarmonyPatch(typeof(PLH), "UpdateVisible")]
    [HarmonyPostfix]
    private static void PLH_UpdateVisible() {
        if (AdditionalModule.alwaysRenderCharactersToggle.GetValue()) {
            for (int i = 0; i < PLH.player.Count; i++) {
                PlayerData data = PLH.player[i];
                if (data == null) continue;
                data.visible = true;
            }
        }
    }
}
