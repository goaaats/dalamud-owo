using System;
using System.Text;
using System.Text.RegularExpressions;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Plugin;

namespace owofy
{
    public class OwoPlugin : IDalamudPlugin
    {
        private DalamudPluginInterface _pi;
        private readonly Random _rng = new Random();

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pi = pluginInterface;
            pluginInterface.Framework.Gui.Chat.OnChatMessage += Chat_OnChatMessage;
        }

        private void Chat_OnChatMessage(Dalamud.Game.Chat.XivChatType type, uint senderId, ref Dalamud.Game.Internal.Libc.StdString sender, ref Dalamud.Game.Internal.Libc.StdString message, ref bool isHandled)
        {
            var parsed = SeString.Parse(message.RawData);

            foreach (var payload in parsed.Payloads)
            {
                if (payload is TextPayload textPayload)
                {
                    textPayload.Text = Owofy(textPayload.Text);
                }
            }

            message.RawData = parsed.Encode();
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

        public string Name => "owo plugin";

        public void Dispose()
        {
            _pi.Framework.Gui.Chat.OnChatMessage -= Chat_OnChatMessage;
            _pi.Dispose();
        }
    }
}
