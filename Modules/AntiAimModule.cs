using TestClient.Interface;
using UnityEngine;

namespace TestClient.Modules; 
internal class AntiAimModule : MonoBehaviour {
    internal static ToggleModule antiAimToggle;
    internal static KeyBindModule antiAimKeyBind;
    internal static DropdownModule antiAimTarget;
    internal static ToggleModule predict;
    internal static ToggleModule oEulerX;
    internal static SliderModule eulerX;
    internal static ToggleModule oEulerY;
    internal static SliderModule eulerY;

    internal static readonly string[] Target = ["None", "Closest", "Closest To Screen", "Exact Player"];

    void Awake() {
        antiAimToggle = ExploitPanel.configurableModules["AntiAim"] as ToggleModule;
        antiAimKeyBind = ExploitPanel.configurableModules["AntiAimKeyBind"] as KeyBindModule;
        antiAimTarget = ExploitPanel.configurableModules["AntiAimTarget"] as DropdownModule;
        predict = ExploitPanel.configurableModules["AAPredict"] as ToggleModule;
        oEulerX = ExploitPanel.configurableModules["AAOEulerX"] as ToggleModule;
        eulerX = ExploitPanel.configurableModules["AAEulerX"] as SliderModule;
        oEulerY = ExploitPanel.configurableModules["AAOEulerY"] as ToggleModule;
        eulerY = ExploitPanel.configurableModules["AAEulerY"] as SliderModule;
    }

    void Update() {
        if (Input.GetKeyDown(antiAimKeyBind.GetValue()))
            antiAimToggle.Toggle.isOn = !antiAimToggle.Toggle.isOn;
    }
}
