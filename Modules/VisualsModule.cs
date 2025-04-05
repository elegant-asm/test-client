using TestClient.Interface;
using UnityEngine;

namespace TestClient.Modules;
internal class VisualsModule : MonoBehaviour {
    internal static ToggleModule espToggle;

    internal static GUIStyle playerNameStyle = new() {
        normal = { textColor = Color.white },
        fontSize = 10,
        fontStyle = FontStyle.Bold
    };
    internal static GUIStyle crouchStateStyle = new() {
        normal = { textColor = Color.yellow },
        fontSize = 8,
        fontStyle = FontStyle.Bold
    };
    internal static GUIStyle protectedStateStyle = new() {
        normal = { textColor = Color.red },
        fontSize = 8,
        fontStyle = FontStyle.Bold
    };

    void Awake() {
        espToggle = ExploitPanel.configurableModules["ESP"] as ToggleModule;
    }

    void OnGUI() {
        if (Plugin.IsRoundStarted && espToggle.GetValue() && Controll.pl != null && Controll.csCam != null) {
            var client = Controll.pl;
            PlayerSync clientSync = Utility.Players.GetPlayerSyncById(client.idx);
            if (clientSync == null)
                return;

            Camera camera = AdditionalModule.thirdPersonToggle.GetValue() ? AdditionalModule.customCamera : Controll.csCam;

            for (int i = 0; i < PLH.player.Length; i++) {
                var player = PLH.player[i];
                if (player == null || player.IsMainPlayer)
                    continue;
                var playerSync = Utility.Players.GetPlayerSyncById(player.idx);
                if (playerSync == null)
                    continue;

                if (!playerSync.IsAlive || player.tr == null || player.rbHead == null)
                    continue;

                Vector3 position = player.tr.position;
                Vector3 position2 = player.rbHead.position;
                Vector3 position3 = new(position.x, position.y - 2.1f, position.z);
                Vector3 position4 = new(position2.x, position2.y + 0.7f, position2.z);

                // ig it should be changed to custom camera if used 3rd person view
                Vector3 screenPos = camera.WorldToScreenPoint(position);
                Vector3 screenPosHead = camera.WorldToScreenPoint(position4);
                Vector3 screenPosFeet = camera.WorldToScreenPoint(position3);

                if (screenPos.z > 0f) {
                    float boxHeight = screenPosHead.y - screenPosFeet.y;
                    float boxWidth = boxHeight / 2f;

                    bool isCrouching = (player.bstate == 2 || player.bstate == 3);

                    float boxX = screenPosFeet.x - (boxWidth / 2f);
                    float boxY = Screen.height - screenPosFeet.y - boxHeight;

                    Color boxColor = Utility.Players.IsInSameTeam(clientSync, playerSync) ? Color.magenta : Color.red;
                    Render.DrawBox(boxX, boxY, boxWidth, boxHeight, boxColor, 1f);

                    GUI.Label(new Rect(boxX, boxY - 12f, 1, 1), player.name, playerNameStyle);
                    //GUI.Label(new Rect(screenPosFeet.x - 15f, Screen.height - screenPosFeet.y + 3f, 100f, 20f), $"{player.health} / 100", guiStyle);

                    if (isCrouching) {
                        GUI.Label(new Rect(boxX + boxWidth + 4f, boxY + 3f, 1 ,1), "CROUCH", crouchStateStyle);
                    }
                    if (player.spawnprotect) {
                        GUI.Label(new Rect(boxX + boxWidth + 4f, boxY + 11f, 1, 1), "PROTECTED", protectedStateStyle);
                    }
                }
            }
        }
    }

    public class Render : MonoBehaviour {
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width) {
            Matrix4x4 matrix = GUI.matrix;
            if (!lineText)
                lineText = new Texture2D(1, 1);

            Color color2 = GUI.color;
            GUI.color = color;
            float num = Vector3.Angle(pointB - pointA, Vector2.right);
            if (pointA.y > pointB.y)
                num = -num;

            GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
            GUIUtility.RotateAroundPivot(num, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), lineText);
            GUI.matrix = matrix;
            GUI.color = color2;
        }

        public static void DrawBox(float x, float y, float w, float h, Color color, float thickness) {
            DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
            DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
            DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
            DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
        }

        public static Texture2D lineText;
    }
}
