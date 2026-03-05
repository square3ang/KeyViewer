using Newtonsoft.Json.Linq;

namespace KeyViewer.Core.Interfaces;

public interface IModel {
    JToken Serialize();
    void Deserialize(JToken node);
}