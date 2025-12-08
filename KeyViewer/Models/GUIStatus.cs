using KeyViewer.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models
{
    public class GUIStatus : IModel, ICopyable<GUIStatus>
    {
        public bool Expanded = false;
        public bool Enabled = true;
        public GUIStatus Copy()
        {
            var status = new GUIStatus();
            status.Expanded = Expanded;
            status.Enabled = Enabled;
            return status;
        }
        public JToken Serialize()
        {
            var node = new JObject();
            node[nameof(Expanded)] = Expanded;
            node[nameof(Enabled)] = Enabled;
            return node;
        }
        public void Deserialize(JToken node)
        {
            var defaultSettings = new GUIStatus();

            Expanded = node[nameof(Expanded)]?.Value<bool>() ?? defaultSettings.Expanded;
            Enabled = node[nameof(Enabled)]?.Value<bool>() ?? defaultSettings.Enabled;
        }
    }
}
