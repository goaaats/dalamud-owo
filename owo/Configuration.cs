using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace owofy
{
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; }
        public bool Enabed { get; set; } = true;

        // Add any other properties or methods here.
        [JsonIgnore] private DalamudPluginInterface pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}
