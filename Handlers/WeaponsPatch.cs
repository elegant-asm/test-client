using HarmonyLib;
using Player;
using TestClient.Modules;
using UnityEngine;

namespace TestClient.Handlers;

[HarmonyPatch]
internal class WeaponsPatch {
    // NoRecoil
    [HarmonyPatch(typeof(Controll), "ChangeWeapon")]
    [HarmonyPostfix]
    private static void ChangeWeapon(int id) {
        //Plugin.Log.LogWarning($"Weapon changed {id}");
        if (WeaponsModule.noRecoilToggle.GetValue())
            WeaponsModule.SetNoRecoil();
    }

    // unforce weapon
    [HarmonyPatch(typeof(PLH), "ForceSelectWeapon")]
    [HarmonyPrefix]
    private static bool ForceSelectWeapon(int id, int wid) {
        return !WeaponsModule.unforceWeaponToggle.GetValue();
    }

    // NoSpread
    [HarmonyPatch(typeof(VWIK), "CameraAddAngle")]
    [HarmonyPrefix]
    private static bool CameraAddAngle(Vector3 v, float timestart, float time, bool reverse = false) {
        return !WeaponsModule.noSpreadToggle.GetValue();
    }
    [HarmonyPatch(typeof(VWIK), "CameraAddOffset")]
    [HarmonyPrefix]
    private static bool CameraAddOffset(Vector3 v, float timestart, float time, bool reverse = false) {
        return !WeaponsModule.noSpreadToggle.GetValue();
    }
    [HarmonyPatch(typeof(VWIK), "AddOffset")]
    [HarmonyPrefix]
    private static bool AddOffset(Vector3 v, float timestart, float time, bool reverse = false) {
        return !WeaponsModule.noSpreadToggle.GetValue();
    }
    [HarmonyPatch(typeof(VWIK), "AddAngle")]
    [HarmonyPrefix]
    private static bool AddAngle(Vector3 v, float timestart, float time, bool reverse = false) {
        return !WeaponsModule.noSpreadToggle.GetValue();
    }
    [HarmonyPatch(typeof(VWIK), "WeaponAddAngle")]
    [HarmonyPrefix]
    private static bool WeaponAddAngle(Vector3 v, float timestart, float time, bool reverse = false) {
        return !WeaponsModule.noSpreadToggle.GetValue();
    }
}
