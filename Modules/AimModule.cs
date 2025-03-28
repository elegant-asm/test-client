using Player;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TestClient.Interface;
using UnityEngine;

namespace TestClient.Modules;

internal class AimModule : MonoBehaviour {
    internal static ToggleModule rageBotToggle;
    internal static DropdownModule rageBotType;
    internal static DropdownModule rageBotCast;
    internal static ToggleModule voxelResolutionToggle;
    internal static SliderModule voxelResolution;
    internal static ToggleModule wideCameraRangeToggle;
    internal static SliderModule wideCameraRange;
    internal static ToggleModule rageBotIgnoreSpawnProtectToggle;
    internal static ToggleModule rageBotIgnoreSelfDeathToggle;
    internal static KeyBindModule rageBotKeyBind;

    internal static readonly string[] rageBotTypes = ["Direct", "Silent"];
    internal static readonly string[] rageBotCasts = ["Camera", "Character"];

    private static readonly List<string> bodyPartPriority = [
        "head", "body", "RightArmUp", "RightArmDown",
            "LeftArmUp", "LeftArmDown", "RightLegUp",
            "RightLegDown", "RightLegBoot", "LeftLegUp",
            "LeftLegDown", "LeftLegBoot"
    ];

    void Awake() {
        rageBotToggle = ExploitPanel.configurableModules["RageBot"] as ToggleModule;
        rageBotType = ExploitPanel.configurableModules["RageBotType"] as DropdownModule;
        rageBotCast = ExploitPanel.configurableModules["RageBotCast"] as DropdownModule;
        voxelResolutionToggle = ExploitPanel.configurableModules["RageBotVoxelResolutionToggle"] as ToggleModule;
        voxelResolution = ExploitPanel.configurableModules["RageBotVoxelResolutionSlider"] as SliderModule;
        wideCameraRangeToggle = ExploitPanel.configurableModules["RageBotWideCameraRangeToggle"] as ToggleModule;
        wideCameraRange = ExploitPanel.configurableModules["RageBotWideCameraRange"] as SliderModule;
        rageBotIgnoreSpawnProtectToggle = ExploitPanel.configurableModules["RageBotIgnoreSpawnProtect"] as ToggleModule;
        rageBotIgnoreSelfDeathToggle = ExploitPanel.configurableModules["RageBotIgnoreSelfDeath"] as ToggleModule;
        rageBotKeyBind = ExploitPanel.configurableModules["RageBotKeyBind"] as KeyBindModule;
    }

    void Update() {
        if (Input.GetKeyDown(rageBotKeyBind.GetValue())) {
            rageBotToggle.Toggle.isOn = !rageBotToggle.GetValue();
        }
    }

    //private static readonly Vector3 cameraSize = new(3f, -3f, 3f);
    //private const float rangeMultiplier = 2f;
    //private static readonly Vector3 centralizeCameraPosition = Vector3.down;
    private static float lastFireTime = 0f;
    internal static IEnumerator RageBotThread() {
        while (rageBotToggle.GetValue()) {
            var client = Controll.pl;
            if (client == null) {
                yield return null;
            }

            PlayerSync clientSync = Utility.Players.GetPlayerSyncById(client.idx);
            if (clientSync == null) {
                yield return null;
            }

            WeaponInv wInv = null;
            WeaponInfo wInfo = null;
            float fireRate = 80;
            bool isWeapon = false;

            if (client.currweapon != null) {
                WeaponData currWeapon = client.currweapon;
                wInfo = GUIInv.GetWeaponInfo(currWeapon.weaponname);
                if (wInfo == null) {
                    yield return null;
                }

                fireRate = wInfo.firerate;
                isWeapon = wInfo.slot == 0 || wInfo.slot == 1 || wInfo.slot == 1000;
                wInv = PLH.GetWeaponInv(client, currWeapon.weaponname);
                if (wInv == null) {
                    yield return null;
                }

                //if (wInfo.firetype == 2) {
                //    Controll.tBurstAttack = Time.time; // need tests cuz idk
                //}

                if (Time.time - lastFireTime < fireRate) {
                    yield return null;
                }
            }

            Camera camera = Controll.csCam;
            if (camera == null) {
                yield return null;
                continue;
            }

            Transform csCam = camera.transform;
            if (csCam == null) {
                yield return null;
                continue;
            }

            if (client.team == 2) {
                yield return null;
                continue;
            }

            var ignoreProtect = rageBotIgnoreSpawnProtectToggle.GetValue();
            var ignoreSelfDeath = rageBotIgnoreSelfDeathToggle.GetValue();

            float lastCameraY = 0f;
            Vector3 cameraPosition = rageBotCast.GetValue() == rageBotCasts[0] ? csCam.position : client.Pos;
            Vector3[] samplePoints = null;
            bool wideRange = wideCameraRangeToggle.GetValue();

            if (wideRange) {
                float range = wideCameraRange.GetValue();
                samplePoints = [
                    cameraPosition,
                    cameraPosition + Vector3.up * range,
                    cameraPosition + Vector3.left * range,
                    cameraPosition + Vector3.right * range,
                    cameraPosition + Vector3.forward * range,
                    cameraPosition + Vector3.back * range,
                    cameraPosition + Vector3.down * range, // better would be even remove it but sometimes good
                ];
            }

            bool isVoxelResolution = voxelResolutionToggle.GetValue();
            int iVoxelResolution = (int)voxelResolution.GetValue();

            for (int i = 0; i < PLH.player.Length; i++) {
                var player = PLH.player[i];
                if (player == null || player.IsMainPlayer)
                    continue;

                PlayerSync playerSync = Utility.Players.GetPlayerSyncById(player.idx);
                if (clientSync == null || playerSync == null || !Utility.Players.AllowedToDamage(clientSync, playerSync, ignoreProtect, ignoreSelfDeath))
                    continue;

                bool triggered = false;
                foreach (var partName in bodyPartPriority) {
                    if (triggered)
                        break;

                    GameObject playerBodyPart = partName switch {
                        "head" => player.goHead,
                        "body" => player.goBody,
                        "RightArmUp" => player.goRArm?[0],
                        "RightArmDown" => player.goRArm?[1],
                        "LeftArmUp" => player.goLArm?[0],
                        "LeftArmDown" => player.goLArm?[1],
                        "RightLegUp" => player.goRLeg?[0],
                        "RightLegDown" => player.goRLeg?[1],
                        "RightLegBoot" => player.goRLeg?[2],
                        "LeftLegUp" => player.goLLeg?[0],
                        "LeftLegDown" => player.goLLeg?[1],
                        "LeftLegBoot" => player.goLLeg?[2],
                        _ => null
                    };

                    if (playerBodyPart == null || playerBodyPart.transform == null)
                        continue;

                    Vector3 bodyPartPosition = playerBodyPart.transform.position;
                    int iterations = wideRange ? samplePoints.Length : 1;

                    for (int j = 0; j < iterations; j++) {
                        Vector3 point = wideRange ? samplePoints[j] : cameraPosition;
                        if (IsBodyPartVisible(point, player.tr, playerBodyPart, out Vector3 hitPoint, isVoxelResolution, iVoxelResolution)) {
                            if (wInfo != null && wInfo.slot == 2 && (point - bodyPartPosition).magnitude > 7)
                                continue;

                            if (wideRange && csCam != null && wInfo != null && wInfo.slot == 2) {
                                lastCameraY = csCam.position.y;
                                csCam.position = point;
                            }

                            if (client.currweapon != null && isWeapon && rageBotType.GetValue() == rageBotTypes[1] && Main.Player != null) {
                                if (wInv.ammo > 0) {
                                    wInv.ammo--;
                                    Main.Player.SetAmmo(wInv.ammo, wInv.ammo);
                                }
                            }

                            lastFireTime = Time.time;

                            if (rageBotType.GetValue() == rageBotTypes[0]) {
                                csCam.LookAt(hitPoint);
                                if (isWeapon) {
                                    Controll.cs.UpdateWeaponAttack();
                                } else if (wInfo != null && wInfo.slot == 2) {
                                    Controll.cs.UpdateShovelAttack();
                                }
                            } else if (rageBotType.GetValue() == rageBotTypes[1]) {
                                var hitData = playerBodyPart.GetComponent<HitData>();
                                if (hitData == null)
                                    continue;

                                Il2CppSystem.Collections.Generic.List<GameClass.AttackData> attackList = new();
                                attackList.Add(new GameClass.AttackData(player.idx, hitData.box, hitPoint));
                                Client.cs.send_attack(point, (uint)Time.time, attackList);
                            }

                            triggered = true;

                            if (lastCameraY != 0f && csCam != null)
                                csCam.position = new Vector3(point.x, lastCameraY, point.z);

                            yield return new WaitForSeconds(fireRate / 1000f);
                            break;
                        }
                    }
                }
            }

            yield return null;
        }
    }

    private static bool IsBodyPartVisible(Vector3 cameraPosition, Transform playerCharacter, GameObject bodyPart, out Vector3 hitPoint, bool isVoxelResolution, int voxelResolution) {
        hitPoint = Vector3.zero;
        Bounds bounds = bodyPart.GetComponent<Renderer>().bounds;
        if (isVoxelResolution) {
            Vector3[] samplePoints = GetSamplePoints(bounds, (int)voxelResolution);

            foreach (var point in samplePoints) {
                if (Physics.Linecast(cameraPosition, point, out RaycastHit hit)) {
                    if (hit.collider.transform.IsChildOf(playerCharacter)) {
                        hitPoint = hit.point;
                        return true;
                    }
                }
            }
        } else {
            if (Physics.Linecast(cameraPosition, bounds.center, out RaycastHit hit)) {
                if (hit.collider.transform.IsChildOf(playerCharacter)) {
                    hitPoint = hit.point;
                    return true;
                }
            }
        }

        return false;
    }

    private static Vector3[] GetSamplePoints(Bounds bounds, int resolution) {
        List<Vector3> samplePoints = [];

        float stepX = bounds.size.x / resolution;
        float stepY = bounds.size.y / resolution;
        float stepZ = bounds.size.z / resolution;

        for (int x = 0; x <= resolution; x += 2)
            for (int y = 0; y <= resolution; y += 2)
                for (int z = 0; z <= resolution; z += 2)
                    samplePoints.Add(bounds.min + new Vector3(x * stepX, y * stepY, z * stepZ));

        return samplePoints.ToArray();
    }
}
