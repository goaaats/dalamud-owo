using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Plugin;
using ImGuiNET;
using owofy.Config;

namespace owofy
{
    public class OwoPlugin : IDalamudPlugin
    {
        private DalamudPluginInterface _pi;
        
        private OwoPluginConfig Config;
        
        private readonly Random _rng = new Random();

        private bool _isMainConfigWindowDrawing;
        
        private readonly HashSet<XivChatType> _playerChatTypes = new HashSet<XivChatType>
        {
            XivChatType.Alliance, XivChatType.Ls1, XivChatType.Ls2, XivChatType.Ls3, XivChatType.Ls4, XivChatType.Ls5,
            XivChatType.Ls6, XivChatType.Ls7, XivChatType.Ls8, XivChatType.Party, XivChatType.Say, XivChatType.Shout,
            XivChatType.Urgent, XivChatType.Yell, XivChatType.CrossParty, XivChatType.FreeCompany,
            XivChatType.NoviceNetwork, XivChatType.TellIncoming, XivChatType.TellOutgoing, XivChatType.CrossLinkShell1,
            XivChatType.CrossLinkShell2, XivChatType.CrossLinkShell3, XivChatType.CrossLinkShell4,
            XivChatType.CrossLinkShell5, XivChatType.CrossLinkShell6, XivChatType.CrossLinkShell7,
            XivChatType.CrossLinkShell8, XivChatType.PvPTeam
        };
        
        private readonly HashSet<XivChatType> _systemMessageChatTypes = new HashSet<XivChatType>
        {
            XivChatType.Debug, XivChatType.Echo, XivChatType.None, XivChatType.Notice, XivChatType.ErrorMessage, 
            XivChatType.RetainerSale, XivChatType.SystemError, XivChatType.SystemMessage, 
            XivChatType.GatheringSystemMessage
        };
        
        private readonly HashSet<XivChatType> _emoteChatTypes = new HashSet<XivChatType>
        {
            XivChatType.CustomEmote, XivChatType.StandardEmote
        };

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pi = pluginInterface;

            Config = pluginInterface.GetPluginConfig() as OwoPluginConfig ?? new OwoPluginConfig();

            _pi.UiBuilder.OnBuildUi += UiBuilder_OnBuildUi;
            _pi.UiBuilder.OnOpenConfigUi += (sender, args) => _isMainConfigWindowDrawing = true;
            
            pluginInterface.Framework.Gui.Chat.OnChatMessage += Chat_OnChatMessage;
        }

        private void Chat_OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (!Config.OwofyPlayerChats && _playerChatTypes.Contains(type) ||
                !Config.OwofySystemMessages && _systemMessageChatTypes.Contains(type) ||
                !Config.OwofyEmotes && _emoteChatTypes.Contains(type))
            {
                return;
            }
            
            foreach (var payload in message.Payloads)
            {
                if (payload is TextPayload textPayload)
                {
                    textPayload.Text = Owofy(textPayload.Text);
                }
            }
        }

        private readonly string[] faces = { "(・`ω´・)", ";;w;;", "owo", "UwU", ">w<", "^w^" };
        private string RndFace() => faces[this._rng.Next(0, faces.Length - 1)];

        private string Owofy(string input)
        {
            input = Regex.Replace(input, "(?:r|l)", "w");
            input = Regex.Replace(input, "(?:R|L)", "W");
            input = Regex.Replace(input, "n([aeiou])", "ny$1");
            input = Regex.Replace(input, "N([aeiou])", "Ny$1");
            input = Regex.Replace(input, "N([AEIOU])", "NY$1");
            input = Regex.Replace(input, "ove", "uv");
            input = Regex.Replace(input, "!+", " " + RndFace() + " ");

            return input;
        }

        private const string _owofyConfigWelcome = "This window allows you to configure Owofy";
        private const string _owofySystemMessage = "Owofy non-player chats (system messages, retainer sale, etc)";
        private const string _owofyPlayerMessage = "Owofy player chats (FC, party, LS, say, etc)";

        private void UiBuilder_OnBuildUi()
        {
            if (!_isMainConfigWindowDrawing) return;
            ImGui.SetNextWindowSize(new Vector2(500, 200));

            if (ImGui.Begin("Owofy Config", ref _isMainConfigWindowDrawing,
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.Text(_owofyConfigWelcome);
                ImGui.Separator();

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(1, 3));
                
                ImGui.Checkbox(Config.OwofySystemMessages ? Owofy(_owofySystemMessage) : _owofySystemMessage,
                    ref Config.OwofySystemMessages);
                ImGui.Checkbox(Config.OwofyPlayerChats ? Owofy(_owofyPlayerMessage) : _owofyPlayerMessage,
                    ref Config.OwofyPlayerChats);
                ImGui.Checkbox(Config.OwofyEmotes ? "Owofy emeowets" : "Owofy emotes", ref Config.OwofyEmotes);
                
                ImGui.PopStyleVar();

                if (ImGui.Button("Save and Close"))
                {
                    _isMainConfigWindowDrawing = false;
                    _pi.SavePluginConfig(Config);
                }
                
                ImGui.End();
            }
        }

        public string Name => "owo plugin";

        public void Dispose()
        {
            _pi.Framework.Gui.Chat.OnChatMessage -= Chat_OnChatMessage;
            _pi.Dispose();
        }
    }
}
