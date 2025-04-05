using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestClient.Interface;
using UnityEngine;

namespace TestClient.Modules;
internal class ChatModule : MonoBehaviour {
    internal static ToggleModule chatFilterToggle;
    internal static DropdownModule chatMessage;
    internal static SliderModule spamMessageDelay;
    internal static ToggleModule spamMessageToggle;
    internal static ToggleModule sendOnKillToggle;

    internal static int MessagesCount = 0;

    internal static readonly string[] filterSymbols = ["█", "░", "\t", "\n", "\r", "\b", "\f", "\v", "  "];

    internal class Message {
        public string Value { get; }
        public string[] Values { get; }
        public float Delay { get; }
        public Message(string value) {
            Value = value;
        }
        public Message(string[] values, float delay) {
            Values = values;
            Delay = delay;
        }
    }
    internal static readonly Dictionary<string, Message> messages = new() {
        { "ClientName",
            new("\n╔╦╗╔═╗░░╔═╗╔╗╔░░╔╦╗╔═╗╔═╗\n░║░║░░░░║░║║║║░░░║░║░║╠═╝\n░╩░╚═╝░░╚═╝╝╚╝░░░╩░╚═╝╩") },
        { "AdDef",
            new("FREE OP CHEAT github,com\\elegant-asm\\test-client") },
        { "AdCenter",
            new("\t\t\t\t\t\t\t\t         FREE OP CHEAT github,com\\elegant-asm\\test-client\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n") },
        { "AdBelowCenter",
            new("\n\n\n\n\t\t\t\t\t\t\t\t         FREE OP CHEAT github,com\\elegant-asm\\test-client") },
        { "BanTrollRus", new("\t\t\t\t\t\t\t\t         Вы будете забанены по подозрению в читерстве\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n") },
        { "BanTrollEng", new("\t\t\t\t\t\t\t\t          You will be banned for suspected cheating\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n") },
        { "WhiteLinesCenter",
            new([
                "\n\t\t\t\t\t\t\t\t          ██████████████████████████████\n\t\t\t\t\t\t\t██████████████████████████████\n\n\n\n\n\n\n\n\n\n\n",
                "\n\t\t\t\t\t\t\t\t          ██████████████████████████████\n\t\t\t\t\t\t\t██████████████████████████████\n\n\n\n\n\n\n\n\n\n\n",
                "\t\t\t\t\t\t\t\t          ███████████████████████████████\n\t\t\t\t\t\t\t███████████████████████████████\n\n\n\n\n\n\n\n\n\n\n",
                "\t\t\t\t\t\t\t\t          ███████████████████████████████\n\t\t\t\t\t\t\t███████████████████████████████\n\n\n\n\n\n\n\n\n\n\n",
                //"\t\t\t\t\t\t\t\t          ███████████████████████████████\n\t\t\t\t\t\t\t███████████████████████████████\n\n\n\n\n\n\n\n\n\n\n"
            ], 0.7f) },
        { "WhiteSquares",
            new("████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████") },
        { "Sosat",
            new("\n█▀▀░█▀█░█▀▀░▄▀▄░▀█▀░█░░\n█░░░█░█░█░░░█▀█░░█░░█▀█\n▀▀▀░▀▀▀░▀▀▀░▀░▀░░▀░░▀▀▀") },
        { "OnKillBaza",
            new("-1 BAZA") }
    };
    internal static readonly string[] messageItems = messages.Keys.ToArray();

    void Awake() {
        chatFilterToggle = ExploitPanel.configurableModules["ChatFilter"] as ToggleModule;
        chatMessage = ExploitPanel.configurableModules["ChatMessage"] as DropdownModule;
        spamMessageDelay = ExploitPanel.configurableModules["SpamMessageDelay"] as SliderModule;
        spamMessageToggle = ExploitPanel.configurableModules["SpamMessage"] as ToggleModule;
        sendOnKillToggle = ExploitPanel.configurableModules["SendOnKill"] as ToggleModule;

        Plugin.OnPlayersClear.OnEvent += (_) => {
            MessagesCount = 0;
        };
    }

    internal static new void SendMessage(string msg) => Client.cs?.send_chatmsg(0, msg);
    internal static IEnumerator SendMessage(string[] msg, float delay) {
        for (int i = 0; i < msg.LongLength; i++) {
            var msgLine = msg[i];
            yield return new WaitForSeconds(delay);
            if (Client.isConnected() && MessagesCount < 9) {
                Client.cs?.send_chatmsg(0, msgLine);
            } else
                break;
        }
    }

    internal static IEnumerator SpamMessage() {
        var choice = chatMessage.GetValue();
        while (spamMessageToggle.GetValue()) {
            if (Client.isConnected() && MessagesCount < 9) {
                var message = messages[choice];
                if (message.Values != null)
                    SendMessage(message.Values, message.Delay);
                else
                    SendMessage(message.Value);
                yield return new WaitForSeconds(spamMessageDelay.GetValue());
                choice = chatMessage.GetValue();
            } else
                yield return new WaitForEndOfFrame();
        }
    }
}
