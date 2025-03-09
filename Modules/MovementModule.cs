using TestClient.Interface;
using UnityEngine;

namespace TestClient.Modules;
internal class MovementModule : MonoBehaviour {
    internal static ToggleModule overrideWalkSpeedToggle;
    internal static SliderModule walkSpeed;
    internal static ToggleModule fakeDuckToggle;
    internal static KeyBindModule fakeDuckKeyBind;
    internal static ToggleModule freeFlyToggle;
    internal static KeyBindModule freeFlyKeyBind;

    void Awake() {
        overrideWalkSpeedToggle = ExploitPanel.configurableModules["OverrideWalkSpeed"] as ToggleModule;
        walkSpeed = ExploitPanel.configurableModules["WalkSpeed"] as SliderModule;
        fakeDuckToggle = ExploitPanel.configurableModules["FakeDuck"] as ToggleModule;
        fakeDuckKeyBind = ExploitPanel.configurableModules["FakeDuckKeyBind"] as KeyBindModule;
        freeFlyToggle = ExploitPanel.configurableModules["FreeFly"] as ToggleModule;
        freeFlyKeyBind = ExploitPanel.configurableModules["FreeFlyKeyBind"] as KeyBindModule;

        Plugin.OnRoundEnd.OnEvent += (_) => {
            if (freeFlyToggle.GetValue())
                freeFlyToggle.Toggle.isOn = false;
        };
    }

    void Update() {
        if (Input.GetKeyDown(freeFlyKeyBind.GetValue()))
            freeFlyToggle.Toggle.isOn = !freeFlyToggle.Toggle.isOn;

        if (Input.GetKeyDown(fakeDuckKeyBind.GetValue()))
            fakeDuckToggle.Toggle.isOn = !fakeDuckToggle.Toggle.isOn;
    }
}
