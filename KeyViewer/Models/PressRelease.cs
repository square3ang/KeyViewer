using JSON;
using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;

namespace KeyViewer.Models
{
    public class PressRelease<T> : IModel, ICopyable<PressRelease<T>>
    {
        public T Pressed;
        public T Released;
        public EaseConfig PressedEase = new EaseConfig();
        public EaseConfig ReleasedEase = new EaseConfig();
        public GUIStatus Status = new GUIStatus();
        public PressRelease() { }
        public PressRelease(T value) => Set(value);
        public PressRelease(T pressed, T released)
        {
            Pressed = pressed;
            Released = released;
        }
        public T Get(bool pressed = true) => pressed ? Pressed : Released;
        public EaseConfig GetEase(bool pressed = true) => pressed ? PressedEase : ReleasedEase;
        public PressRelease<T> Set(T value)
        {
            Pressed = value;
            Released = value;
            return this;
        }
        public PressRelease<T> Set(T pressed, T released)
        {
            Pressed = pressed;
            Released = released;
            return this;
        }
        public PressRelease<T> SetEase(EaseConfig value)
        {
            PressedEase = value;
            ReleasedEase = value;
            return this;
        }
        public PressRelease<T> Copy()
        {
            var newPR = new PressRelease<T>();
            newPR.Pressed = Pressed;
            newPR.Released = Released;
            newPR.PressedEase = PressedEase.Copy();
            newPR.ReleasedEase = ReleasedEase.Copy();
            newPR.Status = Status.Copy();
            return newPR;
        }
        public JsonNode Serialize()
        {
            JsonNode node = JsonNode.Empty;
            node[nameof(Pressed)] = ModelUtils.ToNode<T>(Pressed);
            node[nameof(Released)] = ModelUtils.ToNode<T>(Released);
            node[nameof(PressedEase)] = PressedEase.Serialize();
            node[nameof(ReleasedEase)] = ReleasedEase.Serialize();
            node[nameof(Status)] = Status.Serialize();
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            Pressed = (T)ModelUtils.ToObject<T>(node[nameof(Pressed)]);
            Released = (T)ModelUtils.ToObject<T>(node[nameof(Released)]);
            PressedEase = ModelUtils.Unbox<EaseConfig>(node[nameof(PressedEase)]);
            ReleasedEase = ModelUtils.Unbox<EaseConfig>(node[nameof(ReleasedEase)]);
            Status = ModelUtils.Unbox<GUIStatus>(node[nameof(Status)]) ?? new GUIStatus();
        }
        public bool IsSame => Equals(Pressed, Released);
        public static implicit operator PressRelease<T>(T value) => new PressRelease<T>(value);
    }
    public class PressReleaseM<T> : PressRelease<T>, ICopyable<PressReleaseM<T>> where T : IModel, ICopyable<T>, new()
    {
        public PressReleaseM() { }
        public PressReleaseM(T value) => Set(value);
        public PressReleaseM(T pressed, T released)
        {
            Pressed = pressed;
            Released = released;
        }
        public new PressReleaseM<T> Set(T value)
        {
            Pressed = value;
            Released = value;
            return this;
        }
        public new PressReleaseM<T> Set(T pressed, T released)
        {
            Pressed = pressed;
            Released = released;
            return this;
        }
        public new PressReleaseM<T> SetEase(EaseConfig value)
        {
            PressedEase = value;
            ReleasedEase = value;
            return this;
        }
        public new JsonNode Serialize()
        {
            JsonNode node = JsonNode.Empty;
            node[nameof(Pressed)] = Pressed.Serialize();
            node[nameof(Released)] = Released.Serialize();
            node[nameof(PressedEase)] = PressedEase.Serialize();
            node[nameof(ReleasedEase)] = ReleasedEase.Serialize();
            node[nameof(Status)] = Status.Serialize();
            return node;
        }
        public new void Deserialize(JsonNode node)
        {
            Pressed = ModelUtils.Unbox<T>(node[nameof(Pressed)]);
            Released = ModelUtils.Unbox<T>(node[nameof(Released)]);
            PressedEase = ModelUtils.Unbox<EaseConfig>(node[nameof(PressedEase)]);
            ReleasedEase = ModelUtils.Unbox<EaseConfig>(node[nameof(ReleasedEase)]);
            Status = ModelUtils.Unbox<GUIStatus>(node[nameof(Status)]) ?? new GUIStatus();
        }
        public new PressReleaseM<T> Copy()
        {
            var newPR = new PressReleaseM<T>();
            newPR.Pressed = Pressed.Copy();
            newPR.Released = Released.Copy();
            newPR.PressedEase = PressedEase.Copy();
            newPR.ReleasedEase = ReleasedEase.Copy();
            newPR.Status = Status.Copy();
            return newPR;
        }
        public static implicit operator PressReleaseM<T>(T value) => new PressReleaseM<T>(value);
    }
}
