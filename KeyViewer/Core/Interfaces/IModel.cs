using JSON;

namespace KeyViewer.Core.Interfaces
{
    public interface IModel
    {
        JsonNode Serialize();
        void Deserialize(JsonNode node);
    }
}
