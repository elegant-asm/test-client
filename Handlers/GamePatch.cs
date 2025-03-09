using HarmonyLib;

namespace TestClient.Handlers;

[HarmonyPatch]
internal class GamePatch {
    [HarmonyPatch(typeof(HUD), nameof(HUD.SetMessage))]
    [HarmonyPrefix]
    private static void HUD_SetMessage(string msg, int sec) {
        if (Plugin.IsRoundStarted)
            if (msg == GUIPlay.sRoundLose) {
                //Plugin.Log.LogWarning("Round Lost");
                Plugin.OnRoundEnd.Trigger();
            } else if (msg == GUIPlay.sRoundWon) {
                //Plugin.Log.LogWarning("Round Won");
                Plugin.OnRoundEnd.Trigger();
            } else if (msg == GUIPlay.sHumansWin) {
                //Plugin.Log.LogWarning("Humans Win");
                Plugin.OnRoundEnd.Trigger();
            } else if (msg == GUIPlay.sZombiesWin) {
                //Plugin.Log.LogWarning("Zombies Win");
                Plugin.OnRoundEnd.Trigger();
            }
    }
}
