using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Collections;
using UnityEngine;

namespace TestClient.Components; 

internal class CoroutinesComponent : MonoBehaviour {
    internal static MonoBehaviour _instance;
    //internal delegate IEnumerator RoutineDelegate();

    void Start() {
        _instance = this;
    }

    //internal void StartRoutine(RoutineDelegate enumerator) {
    //    StartCoroutine(enumerator().WrapToIl2Cpp());
    //}
}
