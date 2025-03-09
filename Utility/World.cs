using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using System;
using UnityEngine;

namespace TestClient.Utility;

internal class World {
    public static Component CreateProtectedComponent(Type component, GameObject holder = null, string name = "") {
        ClassInjector.RegisterTypeInIl2Cpp(component);
        if (!ClassInjector.IsTypeRegisteredInIl2Cpp(component)) {
            Plugin.Log.LogInfo($"[CPC]: Registration Fail ThreadClass.");
            return CreateProtectedComponent(component, holder, name);
        }
        //Plugin.Log.LogInfo($"[CPC]: Registration ThreadClass: {ClassInjector.IsTypeRegisteredInIl2Cpp(component)}");

        if (holder == null)
            holder = CreateProtectedHolder(name);
        var _component = holder.AddComponent(Il2CppType.From(component));
        return _component;
    }
    public static Component CreateProtectedComponent<T>(GameObject holder = null, string name = "") where T : MonoBehaviour {
        ClassInjector.RegisterTypeInIl2Cpp<T>();
        if (!ClassInjector.IsTypeRegisteredInIl2Cpp<T>()) {
            Plugin.Log.LogInfo($"[CPC]: Registration Fail ThreadClass.");
            return CreateProtectedComponent<T>(holder, name);
        }
        //Plugin.Log.LogInfo($"[CPC]: Registration ThreadClass: {ClassInjector.IsTypeRegisteredInIl2Cpp(component)}");

        if (holder == null)
            holder = CreateProtectedHolder(name);
        var _component = holder.AddComponent(Il2CppType.Of<T>());
        return _component;
    }

    public static GameObject CreateProtectedHolder(string name = "") {
        name = string.IsNullOrWhiteSpace(name) ? name : Random.RandomString();

        GameObject holder = new(name);
        GameObject.DontDestroyOnLoad(holder);
        holder.hideFlags |= HideFlags.HideAndDontSave;
        return holder;
    }
    public static GameObject CreateProtectedHolder<T>(string name = "") where T : MonoBehaviour {
        name = string.IsNullOrWhiteSpace(name) ? name : Random.RandomString();

        GameObject holder = new(name, Il2CppType.Of<T>());
        GameObject.DontDestroyOnLoad(holder);
        holder.hideFlags |= HideFlags.HideAndDontSave;
        return holder;
    }
}
