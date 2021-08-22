using System;
using System.Text;
using System.Text.RegularExpressions;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using owofy.Attributes;

namespace owofy
{
    public class OwoPlugin : IDalamudPlugin
    {
        private DalamudPluginInterface _pi;
        private PluginCommandManager<OwoPlugin> commandManager;
        private Configuration config;
        private readonly Random _rng = new Random();

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pi = pluginInterface;

            this.config = (Configuration)_pi.GetPluginConfig() ?? new Configuration();
            this.config.Initialize(_pi);

            pluginInterface.Framework.Gui.Chat.OnChatMessage += Chat_OnChatMessage;

            this.commandManager = new PluginCommandManager<OwoPlugin>(this, _pi);
        }

        private void Chat_OnChatMessage(Dalamud.Game.Text.XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (config.Enabed)
            {
                foreach (var payload in message.Payloads)
                {
                    if (payload is TextPayload textPayload)
                    {
                        textPayload.Text = Owofy(textPayload.Text);
                    }
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

        [Command("/owo")]
        [HelpMessage("Turn on owo.")]
        public void OwoCommand(string command, string args)
        {
            config.Enabed = true;
            config.Save();
            // You may want to assign these references to private variables for convenience.
            // Keep in mind that the local player does not exist until after logging in.
            var chat = this._pi.Framework.Gui.Chat;
            chat.Print($"Hello Owofied chat.");
            PluginLog.Verbose("OwO has been enabled.");
        }

        [Command("/uwu")]
        [HelpMessage("Turn off owo.")]
        public void UwuCommand(string command, string args)
        {
            config.Enabed = false;
            config.Save();
            // You may want to assign these references to private variables for convenience.
            // Keep in mind that the local player does not exist until after logging in.
            var chat = this._pi.Framework.Gui.Chat;
            chat.Print($"Goodbye Owofied chat. ;_;");
            PluginLog.Verbose("OwO has been disabled.");
        }

        public string Name => "owo plugin";

        public void Dispose()
        {
            _pi.Framework.Gui.Chat.OnChatMessage -= Chat_OnChatMessage;
            _pi.Dispose();
        }
    }
}
