using TestClient.Interface;
using UnityEngine;

namespace TestClient.Modules;
internal class AntisModule : MonoBehaviour {
    internal static ToggleModule noCameraShakeToggle;
    internal static ToggleModule noStuckStateToggle;
    //internal static ToggleModule airCrouchToggle;

    void Awake() {
        noCameraShakeToggle = ExploitPanel.configurableModules["NoCameraShake"] as ToggleModule;
        noStuckStateToggle = ExploitPanel.configurableModules["NoStuckState"] as ToggleModule;
        //airCrouchToggle = ExploitPanel.configurableModules["AirCrouch"] as ToggleModule;
    }
}