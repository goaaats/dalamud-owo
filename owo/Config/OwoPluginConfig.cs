using Dalamud.Configuration;

namespace owofy.Config
{
    public class OwoPluginConfig : IPluginConfiguration
    {
        public int Version { get; set; }
        
        public bool OwofySystemMessages = true;
        public bool OwofyPlayerChats = true;
        public bool OwofyEmotes = true;
    }
}