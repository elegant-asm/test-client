//using Player;
//using System.Collections.Generic;
//using TestClient.Interface;
//using UnityEngine;

//namespace TestClient.Modules;
//internal class CasesModule : MonoBehaviour {
//    internal static DropdownModule overrideLevelCase;
//    internal class Case(string[] _weapons) {
//        public string[] weapons = _weapons;
//    }

//    internal static readonly Dictionary<string, Case> cases = new() {
//        { "recruit", new(["Default", "SL8", "Mk14 EBR", "M21"]) },
//        { "level", new(["Default", "MAC-10", "MP5K", "Serbu Super-Shorty", "MP7", "Colt 1911", "P90", "Groza", "XM8", "Steyr Scout", "SVD"]) }
//    };

//    void Awake() {
//        overrideLevelCase = ExploitPanel.configurableModules["LevelCaseOverride"] as DropdownModule;
//    }
//}
