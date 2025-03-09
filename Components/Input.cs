using System;
using UnityEngine;

namespace TestClient.Components;

internal class InputComponent : MonoBehaviour {
    internal static KeyCode RegisteredKey;
    private static event System.Action<KeyCode> CallbackKey;

    public static void RegisterKey(System.Action<KeyCode> action) {
        CallbackKey?.Invoke(KeyCode.None);
        CallbackKey = null;
        CallbackKey += action;
        RegisteredKey = KeyCode.None;
    }

    void Update() {
        if (RegisteredKey == KeyCode.None)
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                if (Input.GetKeyDown(keyCode)) {
                    RegisteredKey = keyCode;
                    CallbackKey?.Invoke(keyCode);
                    CallbackKey = null;
                    break;
                }
    }
}
