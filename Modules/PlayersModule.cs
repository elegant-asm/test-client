using BepInEx.Unity.IL2CPP.Utils.Collections;
using Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestClient.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace TestClient.Modules;
internal class PlayersModule : MonoBehaviour {
    internal static DropdownModule targetPlayer;
    internal static OptionModule<string> targetTextPlayer;

    internal static PlayerData targetData;
    internal static PlayerSync targetSync;

    void Awake() {
        targetPlayer = ExploitPanel.configurableModules["TargetPlayer"] as DropdownModule;
        targetTextPlayer = ExploitPanel.configurableModules["TargetTextPlayer"] as OptionModule<string>;

        Plugin.OnPlayerSpawn.OnEvent += (args) => {
            PlayerData playerData = (PlayerData)args[0];
            Thread spawnProtectThread = new Thread(() => {
                Thread.Sleep(5000);
                if (playerData != null)
                    playerData.spawnprotect = false;
            });
            spawnProtectThread.IsBackground = true;
            spawnProtectThread.Start();
        };
        Plugin.OnPlayerJoin.OnEvent += (args) => {
            var data = (PlayerData)args[0];
            if (data.idx == Controll.pl.idx) return;
            targetPlayer.AddOption(data.name);
            string textPlayer = targetTextPlayer.GetValue();
            if (textPlayer != "" && textPlayer == data.name) {
                targetPlayer.Dropdown.value = targetPlayer._items.IndexOf(textPlayer);
            }
        };
        Plugin.OnPlayerLeave.OnEvent += (args) => {
            var data = (PlayerData)args[0];
            targetPlayer.RemoveOption(data.name);
            if (targetData == data) {
                targetData = null;
                targetSync = null;
            }
        };
        Plugin.OnPlayersClear.OnEvent += (_) => {
            targetData = null;
            targetSync = null;
            targetPlayer.ClearOptions();
            targetPlayer.AddOption("None");
            targetPlayer.Dropdown.value = 0;
            targetPlayer.Dropdown.RefreshShownValue();
        };

        StartCoroutine(IUpdatePlayerInfo().WrapToIl2Cpp());
    }

    private static IEnumerator IUpdatePlayerInfo() {
        Dictionary<string, Vector3> previousData = [];
        while (true) {
            if (Plugin.IsRoundStarted) {
                for (int i = 0; i < PLH.player.Length; i++) {
                    var data = PLH.player[i];
                    if (data == null || data.IsMainPlayer) continue;
                    var dataSync = Utility.Players.GetPlayerSyncById(data.idx);
                    if (dataSync == null || !dataSync.IsAlive) continue;
                    if (previousData.ContainsKey(data.name)) {
                        if (previousData[data.name] == data.Pos) {
                            dataSync.MoveDirection = Vector3.zero;
                        } else {
                            Vector3 newPos = (previousData[data.name] - data.Pos).normalized;
                            //Plugin.Log.LogWarning(newPos);
                            dataSync.MoveDirection = -new Vector3(newPos.x, newPos.y, newPos.z);
                        }

                        previousData[data.name] = data.Pos;
                    } else
                        previousData.Add(data.name, data.Pos);
                }
            } else if (previousData.Count > 0)
                previousData.Clear();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
