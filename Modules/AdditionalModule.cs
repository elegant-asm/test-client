using Player;
using System.Collections.Generic;
using TestClient.Interface;
using UnityEngine;

namespace TestClient.Modules;
internal class AdditionalModule : MonoBehaviour {
    internal static SliderModule fov;
    internal static ToggleModule thirdPersonToggle;
    internal static KeyBindModule thirdPersonKeyBind;
    internal static ToggleModule alwaysRenderCharactersToggle;
    internal static ToggleModule instantGrenadeToggle;
    internal static SliderModule grenadeSortRange;
    internal static DropdownModule instantGrenadeKillType;
    internal static ToggleModule ignoreSpawnProtectToggle;

    internal static readonly string[] grenadeKillTypes = ["None", "Random", ">Players", "Exact Player"];

    internal static List<int> grenadeUsedUIDs = [];

    void Awake() {
        fov = ExploitPanel.configurableModules["FOV"] as SliderModule;
        thirdPersonToggle = ExploitPanel.configurableModules["ThirdPerson"] as ToggleModule;
        thirdPersonKeyBind = ExploitPanel.configurableModules["ThirdPersonKeyBind"] as KeyBindModule;
        alwaysRenderCharactersToggle = ExploitPanel.configurableModules["AlwaysRenderCharacters"] as ToggleModule;
        instantGrenadeToggle = ExploitPanel.configurableModules["InstantGrenade"] as ToggleModule;
        grenadeSortRange = ExploitPanel.configurableModules["GrenadeSortRange"] as SliderModule;
        instantGrenadeKillType = ExploitPanel.configurableModules["InstantGrenadeKillType"] as DropdownModule;
        ignoreSpawnProtectToggle = ExploitPanel.configurableModules["InstantGrenadeIgnoreSpawnProtect"] as ToggleModule;

        //Plugin.OnRoundStart.OnEvent += (_) => {
        //    Controll.csCam.fieldOfView = fov.GetValue();
        //};

        Plugin.OnPlayersClear.OnEvent += (_) => {
            grenadeUsedUIDs.Clear();
        };
    }

    internal static GameObject customCameraObj = null;
    internal static Camera customCamera = null;
    void Start() {
        customCameraObj = new GameObject("MainCamera");
        DontDestroyOnLoad(customCameraObj);
        customCameraObj.hideFlags = HideFlags.HideAndDontSave;

        customCamera = customCameraObj.AddComponent<Camera>();
        customCamera.enabled = false;
        customCamera.transform.position = new Vector3(0, 0, -10);
        customCamera.clearFlags = CameraClearFlags.Skybox;
        customCamera.fieldOfView = fov.GetValue();

        Material darkSkybox = new Material(Shader.Find("Skybox/Procedural"));
        darkSkybox.SetColor("_SkyTint", Color.black);
        darkSkybox.SetFloat("_AtmosphereThickness", 0f);
        darkSkybox.SetColor("_GroundColor", Color.black);

        RenderSettings.skybox = darkSkybox;
    }

    void Update() {
        if (Input.GetKeyDown(thirdPersonKeyBind.GetValue()))
            thirdPersonToggle.Toggle.isOn = !thirdPersonToggle.Toggle.isOn;

        if (Controll.csCam != null)
            Controll.csCam.fieldOfView = fov.GetValue(); // some glitches when shoot but idc

        if (thirdPersonToggle.GetValue() && Plugin.IsGameInProgress) {
            if (Controll.csRadarCam)
                Controll.csRadarCam.enabled = false;
            if (Controll.csRadarCam2)
                Controll.csRadarCam2.enabled = false;
            if (Controll.csCam)
                Controll.csCam.enabled = false;

            for (int i = 0; i < PLH.player.Count; i++) { // i tried to optimize put to event but it for some reason don't set it to false even on spawn
                PlayerData player = PLH.player[i];
                if (player != null && player.goArrow != null)
                    player.goArrow.SetActive(false);
            }

            foreach (Transform child in Controll.trControll.GetComponentsInChildren<Transform>()) {
                if (child.name.StartsWith("Arrow")) {
                    child.gameObject.SetActive(false);
                    break;
                }
            }

            Vector3 forwardDirection = Controll.csCam.transform.forward;
            Vector3 newPosition = Controll.csCam.transform.position;
            Vector3 charNewPosition = Vector3.Lerp(Controll.pl.tr.position, Controll.Pos + Vector3.up * 2f, 0.25f);
            newPosition -= forwardDirection * 3f;
            newPosition.y += 1f;
            Controll.pl.tr.position = charNewPosition;
            customCamera.transform.position = newPosition;
            customCamera.transform.eulerAngles = Controll.csCam.transform.eulerAngles;
        }
    }
}
