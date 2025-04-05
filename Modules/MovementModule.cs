using TestClient.Interface;
using UnityEngine;
using static TestClient.Interface.ExploitPanel.Movement;

namespace TestClient.Modules;
internal class MovementModule : MonoBehaviour {
    internal static ToggleModule betterMovementToggle;
    internal static ToggleModule overrideWalkSpeedToggle;
    internal static SliderModule walkSpeed;
    internal static ToggleModule fakeDuckToggle;
    internal static KeyBindModule fakeDuckKeyBind;
    internal static ToggleModule freeFlyToggle;
    internal static KeyBindModule freeFlyKeyBind;
    internal static ToggleModule autoJumpToggle;
    internal static ToggleModule targetStrafeToggle;
    internal static KeyBindModule targetStrafeKeyBind;
    internal static ToggleModule targetStrafeIgnoreSpawnProtect;
    internal static DropdownModule targetStrafeType;
    internal static SliderModule targetStrafeDistance;
    internal static SliderModule targetStrafeYLimit;
    internal static SliderModule targetStrafeSpeed;
    internal static SliderModule targetStrafeDistanceTrigger;

    internal static readonly string[] targetStrafeTypes = ["Closest", "Exact Target"];

    void Awake() {
        betterMovementToggle = ExploitPanel.configurableModules["BetterMovement"] as ToggleModule;
        overrideWalkSpeedToggle = ExploitPanel.configurableModules["OverrideWalkSpeed"] as ToggleModule;
        walkSpeed = ExploitPanel.configurableModules["WalkSpeed"] as SliderModule;
        fakeDuckToggle = ExploitPanel.configurableModules["FakeDuck"] as ToggleModule;
        fakeDuckKeyBind = ExploitPanel.configurableModules["FakeDuckKeyBind"] as KeyBindModule;
        freeFlyToggle = ExploitPanel.configurableModules["FreeFly"] as ToggleModule;
        freeFlyKeyBind = ExploitPanel.configurableModules["FreeFlyKeyBind"] as KeyBindModule;
        autoJumpToggle = ExploitPanel.configurableModules["AutoJump"] as ToggleModule;
        targetStrafeToggle = ExploitPanel.configurableModules["TargetStrafe"] as ToggleModule;
        targetStrafeKeyBind = ExploitPanel.configurableModules["TargetStrafeKeyBind"] as KeyBindModule;
        targetStrafeIgnoreSpawnProtect = ExploitPanel.configurableModules["TargetStrafeIgnoreSpawnProtect"] as ToggleModule;
        targetStrafeType = ExploitPanel.configurableModules["TargetStrafeType"] as DropdownModule;
        targetStrafeDistance = ExploitPanel.configurableModules["TargetStrafeDistance"] as SliderModule;
        targetStrafeYLimit = ExploitPanel.configurableModules["TargetStrafeYLimit"] as SliderModule;
        targetStrafeSpeed = ExploitPanel.configurableModules["TargetStrafeSpeed"] as SliderModule;
        targetStrafeDistanceTrigger = ExploitPanel.configurableModules["TargetStrafeDistanceTrigger"] as SliderModule;

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

        if (Input.GetKeyDown(targetStrafeKeyBind.GetValue()))
            targetStrafeToggle.Toggle.isOn = !targetStrafeToggle.Toggle.isOn;

        if (Controll.cs != null && !Controll.inAir)
            Controll.inJumpKey = true;
    }
}
