using System.Collections.Generic;
using TestClient.Interface;
using UnityEngine;

namespace TestClient.Modules;
internal class AdditionalModule : MonoBehaviour {
    internal static SliderModule grenadeSortRange;
    internal static DropdownModule instantGrenadeKill;
    internal static ToggleModule instantGrenadeToggle;
    internal static ToggleModule ignoreSpawnProtectToggle;

    internal static readonly string[] grenadeKillType = ["None", "Random", ">Players", "Exact Player"];

    internal static List<int> grenadeUsedUIDs = [];

    void Awake() {
        grenadeSortRange = ExploitPanel.configurableModules["GrenadeSortRange"] as SliderModule;
        instantGrenadeKill = ExploitPanel.configurableModules["InstantGrenadeKill"] as DropdownModule;
        instantGrenadeToggle = ExploitPanel.configurableModules["InstantGrenade"] as ToggleModule;
        ignoreSpawnProtectToggle = ExploitPanel.configurableModules["InstantGrenadeIgnoreSpawnProtect"] as ToggleModule;

        Plugin.OnPlayersClear.OnEvent += (_) => {
            grenadeUsedUIDs.Clear();
        };
    }
}
