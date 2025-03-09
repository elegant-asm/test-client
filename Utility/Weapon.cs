using Player;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniverseLib;

namespace TestClient.Utility;
internal static class Weapon {
    internal static List<DefaultWeapon> weapons = [];
    internal class WeaponProperty<T>(string _name, T _default, WeaponInfo _wInfo) {
        public readonly string name = _name;
        public readonly T _default = _default;
        private T value = _default;

        private readonly WeaponInfo wInfo = _wInfo;
        private PropertyInfo property;
        public void SetValue(T _value) {
            if (property == null)
                property = wInfo.GetActualType().GetProperty(name);
            property.SetValue(wInfo, _value);
            value = _value;
        }
        public T GetValue() => value;
        public void ResetValue() {
            if (property == null)
                property = wInfo.GetActualType().GetProperty(name);
            property.SetValue(wInfo, _default);
        }
        public void RecoverValue() {
            if (property == null)
                property = wInfo.GetActualType().GetProperty(name);
            property.SetValue(wInfo, value);
        }
        public void ClearValue() {
            ResetValue();
            value = _default;
        }
    }
    internal class DefaultWeapon {
        public WeaponInfo wInfo;

        public WeaponProperty<int> firerate;
        public WeaponProperty<int> recoil;
        public WeaponProperty<int> accuracy;
        //public WeaponProperty<int> firetype;

        public DefaultWeapon(WeaponInfo _wInfo) {
            weapons.Add(this);
            wInfo = _wInfo;

            firerate = new("firerate", wInfo.firerate, wInfo);
            recoil = new("recoil", wInfo.recoil, wInfo);
            accuracy = new("accuracy", wInfo.accuracy, wInfo);
            //firetype = new("firetype", wInfo.firetype, wInfo);
        }
    }

    internal static DefaultWeapon Get(WeaponInfo wInfo) {
        return weapons.FirstOrDefault(_weapon => _weapon.wInfo.name == wInfo.name) ?? new(wInfo);
    }
    internal static DefaultWeapon Get(string wName) {
        return weapons.FirstOrDefault(_weapon => _weapon.wInfo.name == wName) ?? new(GUIInv.GetWeaponInfo(wName));
    }

    internal static void Reset<T>(string name) {
        for (int i = 0; i < weapons.Count; i++) {
            var weapon = weapons[i];
            var weaponProperty = (WeaponProperty<T>)weapon.GetActualType().GetField(name).GetValue(weapon);
            weaponProperty.ResetValue();
        }
    }
    internal static void Reset() {
        for (int i = 0; i < weapons.Count; i++) {
            var weapon = weapons[i];
            weapon.firerate.ResetValue();
            weapon.recoil.ResetValue();
            weapon.accuracy.ResetValue();
            //weapon.firetype.ResetValue();
        }
    }

    internal static void Recover<T>(string name) {
        for (int i = 0; i < weapons.Count; i++) {
            var weapon = weapons[i];
            var weaponProperty = (WeaponProperty<T>)weapon.GetActualType().GetField(name).GetValue(weapon);
            weaponProperty.RecoverValue();
        }
    }
    internal static void Recover() {
        for (int i = 0; i < weapons.Count; i++) {
            var weapon = weapons[i];
            weapon.firerate.RecoverValue();
            weapon.recoil.RecoverValue();
            weapon.accuracy.RecoverValue();
            //weapon.firetype.RecoverValue(); // if something be wrong, if in this game exist firetype switch, remove it line.
        }
    }

    internal static void Clear<T>(string name) {
        //Reset<T>(name);
        for (int i = 0; i < weapons.Count; i++) {
            var weapon = weapons[i];
            var weaponProperty = (WeaponProperty<T>)weapon.GetActualType().GetField(name).GetValue(weapon);
            weaponProperty.ClearValue();
        }
    }

    internal static void Clear() {
        Reset();
        //weapons.Clear();
    }
}