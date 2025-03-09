using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Collections;
using TestClient.Interface;
using UnityEngine;

namespace TestClient.Modules;

internal class WeaponsModule : MonoBehaviour {
    internal static ToggleModule autoReloadToggle;
    internal static ToggleModule fastReloadToggle;
    internal static ToggleModule noSpreadToggle;
    internal static ToggleModule noRecoilToggle;
    internal static ToggleModule unforceWeaponToggle;
    internal static ToggleModule alwaysRegisterAsHead;
    //internal static ToggleModule shootInReloadToggle;
    internal static SliderModule gunFireRate;
    internal static SliderModule gunFireType;
    internal static SliderModule fastReloadDelay;
    internal static SliderModule damageMultiplier;

    void Awake() {
        autoReloadToggle = ExploitPanel.configurableModules["AutoReload"] as ToggleModule;
        fastReloadToggle = ExploitPanel.configurableModules["FastReload"] as ToggleModule;
        noSpreadToggle = ExploitPanel.configurableModules["NoSpread"] as ToggleModule;
        noRecoilToggle = ExploitPanel.configurableModules["NoRecoil"] as ToggleModule;
        unforceWeaponToggle = ExploitPanel.configurableModules["UnforceWeapon"] as ToggleModule;
        alwaysRegisterAsHead = ExploitPanel.configurableModules["AlwaysRegisterAsHead"] as ToggleModule;
        //shootInReloadToggle = ExploitPanel.configurableModules["ShootInReload"] as ToggleModule;
        gunFireRate = ExploitPanel.configurableModules["GunFireRate"] as SliderModule;
        gunFireType = ExploitPanel.configurableModules["GunFireType"] as SliderModule;
        fastReloadDelay = ExploitPanel.configurableModules["FastReloadDelay"] as SliderModule;
        damageMultiplier = ExploitPanel.configurableModules["DamageMultiplier"] as SliderModule;

        //Plugin.OnRoundStart.OnEvent += (_) => Utility.Weapon.Recover();
        //Plugin.OnPlayersClear.OnEvent += (_) => Utility.Weapon.Reset();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R) && fastReloadToggle.GetValue()) {
            StartCoroutine(FastReload().WrapToIl2Cpp());
        }

        if (fastReloadToggle.GetValue() && Plugin.IsRoundStarted && Controll.inReload && UIAmmo.Instance != null && UIAmmo.Instance._reloadingProgress != null) {
            Controll.reload_active = UIAmmo.Instance._reloadingProgress.fillAmount;
        }
    }

    internal static void SetNoRecoil() {
        if (Controll.pl != null && Controll.pl.currweapon != null) {
            var defaultWeapon = Utility.Weapon.Get(Controll.pl.currweapon.weaponname);
            defaultWeapon.recoil.SetValue(0);
            defaultWeapon.accuracy.SetValue(100);
        }
    }

    internal static IEnumerator FastReload(bool secondPhase = true) {
        if (!secondPhase)
            Controll.ReloadWeapon();
        yield return new WaitForSeconds(fastReloadDelay.GetValue()); // maybe I can make reload faster? (<500 not registering, 550 is kinda good but sometime not reg, 710 is perfect)
        Controll.ReloadWeapon();
    }
}
