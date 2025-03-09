using HarmonyLib;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

namespace TestClient.Handlers;
[HarmonyPatch]
internal class SystemPatch {
    private static System.Random _rand = new();

    private static string GetRandomString(int length) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] stringChars = new char[length];
        for (int i = 0; i < stringChars.Length; i++) {
            stringChars[i] = chars[_rand.Next(chars.Length)];
        }
        return new string(stringChars);
    }

    private static string GetRandomMacAddress() {
        return string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                             _rand.Next(0, 256),
                             _rand.Next(0, 256),
                             _rand.Next(0, 256),
                             _rand.Next(0, 256),
                             _rand.Next(0, 256),
                             _rand.Next(0, 256));
    }

    [HarmonyPatch(typeof(SystemInfo), "deviceModel", MethodType.Getter)]
    public class PatchDeviceModel {
        static bool Prefix(ref string __result) {
            __result = "Device-" + GetRandomString(6);
            Plugin.Log.LogWarning("device model check");
            return false;
        }
    }

    [HarmonyPatch(typeof(SystemInfo), "operatingSystem", MethodType.Getter)]
    public class PatchOperatingSystem {
        static bool Prefix(ref string __result) {
            __result = "Windows 10 " + _rand.Next(1, 10) + ".0" + _rand.Next(1, 10);
            Plugin.Log.LogWarning("operating system check");
            return false;
        }
    }

    [HarmonyPatch(typeof(SystemInfo), "graphicsDeviceName", MethodType.Getter)]
    public class PatchGraphicsDeviceName {
        static bool Prefix(ref string __result) {
            __result = "NVIDIA GeForce GTX " + _rand.Next(600, 1000);
            Plugin.Log.LogWarning("graphics device name check");
            return false;
        }
    }

    //[HarmonyPatch(typeof(SystemInfo), "systemMemorySize", MethodType.Getter)]
    //public class PatchSystemMemorySize {
    //    static bool Prefix(ref int __result) {
    //        __result = _rand.Next(8192, 16384);
    //        Plugin.Log.LogWarning("system memory size check");
    //        return false;
    //    }
    //}

    [HarmonyPatch(typeof(SystemInfo), "deviceUniqueIdentifier", MethodType.Getter)]
    public class PatchDeviceUniqueIdentifier {
        static bool Prefix(ref string __result) {
            __result = "DEV-" + GetRandomString(16);
            Plugin.Log.LogWarning("device unique identifier check");
            return false;
        }
    }

    [HarmonyPatch(typeof(SystemInfo), "processorType", MethodType.Getter)]
    public class PatchProcessorType {
        static bool Prefix(ref string __result) {
            __result = "Intel Core i7-" + _rand.Next(7000, 9000);
            Plugin.Log.LogWarning("processor type check");
            return false;
        }
    }

    //[HarmonyPatch(typeof(SystemInfo), "processorFrequency", MethodType.Getter)]
    //public class PatchProcessorFrequency {
    //    static bool Prefix(ref int __result) {
    //        __result = _rand.Next(2500, 4000);
    //        Plugin.Log.LogWarning("processor frequency check");
    //        return false;
    //    }
    //}

    [HarmonyPatch(typeof(NetworkInterface), "GetPhysicalAddress")]
    public class SpoofMacAddress {
        static bool Prefix(ref PhysicalAddress __result) {
            string mac = GetRandomMacAddress();
            __result = new PhysicalAddress(
                mac.Split(':').Select(s => Convert.ToByte(s, 16)).ToArray());
            Plugin.Log.LogWarning("physical addr check");
            return false;
        }
    }

    //[HarmonyPatch(typeof(SystemInfo), "operatingSystemFamily", MethodType.Getter)]
    //public class PatchSystemArchitecture {
    //    static bool Prefix(ref OperatingSystemFamily __result) {
    //        __result = _rand.Next(0, 2) == 0 ? OperatingSystemFamily.Windows : OperatingSystemFamily.MacOSX;
    //        Plugin.Log.LogWarning("system architecture check");
    //        return false;
    //    }
    //}

    [HarmonyPatch(typeof(SystemInfo), "deviceName", MethodType.Getter)]
    public class PatchDeviceName {
        static bool Prefix(ref string __result) {
            __result = "Device-" + GetRandomString(6);
            Plugin.Log.LogWarning("device name check");
            return false;
        }
    }

    [HarmonyPatch(typeof(SystemInfo), "graphicsMemorySize", MethodType.Getter)]
    public class PatchGraphicsMemorySize {
        static bool Prefix(ref int __result) {
            __result = _rand.Next(2048, 8192);
            Plugin.Log.LogWarning("graphics memory size check");
            return false;
        }
    }

    //[HarmonyPatch(typeof(Screen), "width", MethodType.Getter)]
    //public class PatchScreenWidth {
    //    static bool Prefix(ref int __result) {
    //        __result = _rand.Next(1920, 3840);
    //        Plugin.Log.LogWarning("screen width check");
    //        return false;
    //    }
    //}

    //[HarmonyPatch(typeof(Screen), "height", MethodType.Getter)]
    //public class PatchScreenHeight {
    //    static bool Prefix(ref int __result) {
    //        __result = _rand.Next(1080, 2160);
    //        Plugin.Log.LogWarning("screen height check");
    //        return false;
    //    }
    //}

    //[HarmonyPatch(typeof(SystemInfo), "language", MethodType.Getter)]
    //public class PatchSystemLanguage {
    //    static bool Prefix(ref string __result) {
    //        __result = "en-US";
    //        Plugin.Log.LogWarning("system language check");
    //        return false;
    //    }
    //}
}
