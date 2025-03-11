using System;
using System.Linq;
using TestClient.Utility;
using UnityEngine;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace TestClient.Interface;

internal class Manager : MonoBehaviour {
    private static bool _initialized = false;

    private const KeyCode _showUIKeyCode = KeyCode.F5;

    private static bool _showUI = true;

    internal static UIBase UiBase { get; private set; }
    public static GameObject UIRoot => UiBase.RootObject;
    public static RectTransform UIRootRect { get; private set; }
    public static Canvas UICanvas { get; private set; }

    void Start() {
        UniverseLib.Config.UniverseLibConfig config = new() {
            Disable_EventSystem_Override = false,
            Force_Unlock_Mouse = false
        };

        Universe.Init(1f, OnUniverseInit, LogHandler, config);

        _initialized = true;
    }

    void Update() {
        if (Input.GetKeyDown(_showUIKeyCode)) {
            _showUI = !_showUI;
            UniversalUI.SetUIActive(MyPluginInfo.PLUGIN_GUID, _showUI);
        }
    }

    private void OnUniverseInit() {
        if (UniverseLib.Input.EventSystemHelper.CurrentEventSystem) {
            UniverseLib.Input.EventSystemHelper.CurrentEventSystem.sendNavigationEvents = false; // to prevent idiotic miss clicks by my keyboard
        }
        UiBase = UniversalUI.RegisterUI(MyPluginInfo.PLUGIN_GUID, UIUpdate);
        UICanvas = UIRoot.GetComponent<Canvas>();
        UIRootRect = UIRoot.GetComponent<RectTransform>();

        new ExploitPanel(UiBase);
    }

    
    private void UIUpdate() {
        if (UniverseLib.Input.EventSystemHelper.CurrentEventSystem && UniverseLib.Input.EventSystemHelper.CurrentEventSystem.sendNavigationEvents == true) {
            UniverseLib.Input.EventSystemHelper.CurrentEventSystem.sendNavigationEvents = false; // universelib loads eventsystem somewhy slow
        }
    }

    private void LogHandler(string message, LogType type) {
        Plugin.Log.LogWarning($"[{type}] UniverseLib: {message}");
    }
}