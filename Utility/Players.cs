using Player;
using System.Linq;

namespace TestClient.Utility;
internal class Players {
    internal static PlayerData GetPlayerById(int id) {
        for (int i = 0; i < PLH.player.Length; i++) {
            PlayerData player = PLH.player[i];
            if (player == null) continue;
            if (player.idx == id)
                return player;
        }
        Plugin.Log.LogWarning($"Player with id {id} not found in player list.");
        return null;
    }
    internal static PlayerSync GetPlayerSyncById(int id) {
        return Plugin.Players.FirstOrDefault(_player => _player.data != null && _player.data.idx == id);
    }
    internal static bool AllowedToDamage(PlayerSync clientSync, PlayerSync playerSync, bool ignoreProtect = false, bool ignoreSelfDeath = false) {
        return !((!ignoreSelfDeath && !clientSync.IsAlive) || !playerSync.IsAlive || IsInSameTeam(clientSync, playerSync) || (!ignoreProtect && playerSync.data.spawnprotect));
    }
    internal static bool IsInSameTeam(PlayerSync clientSync, PlayerSync playerSync) {
        return (Controll.gamemode == 5 && playerSync.data.team == clientSync.data.team && playerSync.IsZombie == clientSync.IsZombie) ||
                (Controll.gamemode != 5 && !Plugin.Deathmatches.Contains(Controll.gamemode) && playerSync.data.team == clientSync.data.team);
    }
    //internal static bool IsClient(PlayerData data) {
    //    return data.IsMainPlayer;
    //}
}
