using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class PressRelease<T> : IModel
    {
        public T Pressed;
        public T Released;
        public PressRelease() { }
        public PressRelease(T value) => Set(value);
        public PressRelease(T pressed, T released)
        {
            Pressed = pressed;
            Released = released;
        }
        public T Get(bool pressed = true) => pressed ? Pressed : Released;
        public void Set(T value)
        {
            Pressed = value;
            Released = value;
        }
        public PressRelease<T> Copy()
        {
            var newPR = new PressRelease<T>();
            newPR.Pressed = Pressed;
            newPR.Released = Released;
            return newPR;
        }
        public JsonNode Serialize()
        {
            JsonNode node = JsonNode.Empty;
            node[nameof(Pressed)] = ModelUtils.ToNode<T>(Pressed);
            node[nameof(Released)] = ModelUtils.ToNode<T>(Released);
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Pressed = (T)ModelUtils.ToObject<T>(node[nameof(Pressed)]);
            Released = (T)ModelUtils.ToObject<T>(node[nameof(Released)]);
        }
        public bool IsSame => Equals(Pressed, Released);
        public static implicit operator PressRelease<T>(T value) => new PressRelease<T>(value);
    }
    public class PressReleaseM<T> : PressRelease<T> where T : IModel, new()
    {
        public PressReleaseM() { }
        public PressReleaseM(T value) => Set(value);
        public PressReleaseM(T pressed, T released)
        {
            Pressed = pressed;
            Released = released;
        }
        public new JsonNode Serialize()
        {
            JsonNode node = JsonNode.Empty;
            node[nameof(Pressed)] = Pressed.Serialize();
            node[nameof(Released)] = Released.Serialize();
            return node;
        }
        public new void Deserialize(JsonNode node)
        {
            T p = new T();
            p.Deserialize(node[nameof(Pressed)]);
            Pressed = p;

            T r = new T();
            r.Deserialize(node[nameof(Released)]);
            Released = r;
        }
        public new PressReleaseM<T> Copy()
        {
            var newPR = new PressReleaseM<T>();
            newPR.Pressed = Pressed;
            newPR.Released = Released;
            return newPR;
        }
        public static implicit operator PressReleaseM<T>(T value) => new PressReleaseM<T>(value);
    }
}
