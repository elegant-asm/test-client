using Player;
using System.Linq;

namespace TestClient.Utility;
internal class Players {
    internal static PlayerData GetPlayerById(int id) {
        PlayerData playerData = PLH.player.FirstOrDefault(_player => _player != null && _player.idx == id);
        if (playerData == null)
            Plugin.Log.LogWarning($"PlayerData with id {id} not found.");
        return playerData;
    }
    internal static PlayerSync GetPlayerSyncById(int id) {
        PlayerSync playerData = Plugin.Players.FirstOrDefault(_player => _player.data != null && _player.data.idx == id);
        //if (playerData == null)
        //    Plugin.Log.LogWarning($"PlayerSync with id {id} not found.");
        return playerData;
    }
    internal static bool AllowedToDamage(PlayerSync clientSync, PlayerSync playerSync, bool ignoreProtect = false, bool ignoreSelfDeath = false) {
        return !((!ignoreSelfDeath && !clientSync.IsAlive) || !playerSync.IsAlive || IsInSameTeam(clientSync, playerSync) || (!ignoreProtect && playerSync.data.spawnprotect));
    }
    internal static bool IsInSameTeam(PlayerSync clientSync, PlayerSync playerSync) {
        return (Controll.gamemode == 5 && playerSync.data.team == clientSync.data.team && playerSync.IsZombie == clientSync.IsZombie) ||
                (Controll.gamemode != 5 && !Plugin.Deathmatches.Contains(Controll.gamemode) && playerSync.data.team == clientSync.data.team);
    }
}