using Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TestClient.Interface;

internal class PlayersList : MonoBehaviour {
    private static GameObject panelObj;
    private static Text text;

    private static List<string> playerMods = ["АнечкаЛиванская"];

    void Start() {
        GameObject canvasObj = new("PlayersList");
        DontDestroyOnLoad(canvasObj);
        canvasObj.hideFlags = HideFlags.HideAndDontSave;
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        panelObj = new("PlayersPanel");
        panelObj.transform.SetParent(canvas.transform);

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.65f);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(10, -50);
        panelRect.sizeDelta = new Vector2(350, 0);

        ContentSizeFitter sizeFitter = panelObj.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        VerticalLayoutGroup layoutGroup = panelObj.AddComponent<VerticalLayoutGroup>();
        //layoutGroup.spacing = 5;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        GameObject textObj = new("PlayersText");
        textObj.transform.SetParent(panelObj.transform);

        text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 14;
        text.supportRichText = true;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
    }

    void Update() {
        string players = "";
        for (var i = 0; i < PLH.player.Length; i++) {
            PlayerData data = PLH.player[i];
            if (data == null) continue;

            PlayerSync playerSync = Utility.Players.GetPlayerSyncById(data.idx);
            if (playerSync == null) continue;

            string playerColor = "<color=#{0}>";
            if (data.IsMainPlayer)
                playerColor = String.Format(playerColor, "BF77F6");
            else if (playerSync.IsZombie)
                playerColor = String.Format(playerColor, "0EAB1D");
            else if (data.team == 0)
                playerColor = String.Format(playerColor, "EA2113");
            else if (data.team == 1)
                playerColor = String.Format(playerColor, "0165FC");

            string playerText = $"{playerColor}{data.name}</color> [{data.idx}] [{data.sLVL}] ({data.team})";
            if (playerSync.IsAlive)
                playerText += " <color=#C2D422>[ALIVE]</color>";
            else
                playerText += " <color=#D61327>[DEAD]</color>";

            if (playerMods.Contains(data.name))
                playerText += " <color=#FF991C>[MOD]</color>";

            if (data.team == 2)
                playerText += " [SPECTATOR]";
            //if (playerSync.IsZombie)
            //    playerText += " <color=#0eab1d>[ZOMBIE]</color>";
            players += playerText + "\n";
        }
        if (players.Length > 0)
            players = players[..^1];
        text.text = players;
    }

    //internal static void AddPlayer(PlayerData playerData) {
    //    PlayerData player = GetPlayerById(playerData.idx);
    //    if (player != null)
    //        return;

    //    playersList.AddItem(player);
    //}
    //internal static void RemovePlayer(int id) {
    //    PlayerData player = GetPlayerById(id);
    //    if (player == null)
    //        return;

    //    playersList.Remove(player);
    //}
    //internal static void Clear() {
    //    playersList.Clear();
    //}
}
