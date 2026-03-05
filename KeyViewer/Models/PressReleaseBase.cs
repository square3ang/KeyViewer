using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models;

public class PressReleaseBase<T> : IModel, ICopyable<PressReleaseBase<T>> {
    public T Pressed;
    public T Released;
    public PressReleaseBase() { }
    public PressReleaseBase(T value) => Set(value);
    public PressReleaseBase(T pressed, T released) {
        Pressed = pressed;
        Released = released;
    }
    public T Get(bool pressed = true) => pressed ? Pressed : Released;
    public PressReleaseBase<T> Set(T value) {
        Pressed = value;
        Released = value;
        return this;
    }
    public PressReleaseBase<T> Set(T pressed, T released) {
        Pressed = pressed;
        Released = released;
        return this;
    }
    public PressReleaseBase<T> Copy() {
        return new PressReleaseBase<T> {
            Pressed = Pressed,
            Released = Released
        };
    }

    public virtual JToken Serialize() {
        var node = new JObject();
        if(Released != null) {
            node[nameof(Released)] = ModelUtils.ToNode<T>(Released);
        }
        if(Pressed != null && !IsSame) {
            node[nameof(Pressed)] = ModelUtils.ToNode<T>(Pressed);
        }
        return node;
    }
    public virtual void Deserialize(JToken node) {
        JToken releasedRaw = node[nameof(Released)];
        JToken pressedRaw = node[nameof(Pressed)];
        bool nullReleased = releasedRaw == null;
        if(!nullReleased) {
            Released = (T)ModelUtils.ToObject<T>(releasedRaw);
        }
        Pressed = pressedRaw == null ? nullReleased ? default : Released : (T)ModelUtils.ToObject<T>(pressedRaw);
    }
    public bool IsSame => Equals(Pressed, Released);
    public static implicit operator PressReleaseBase<T>(T value) => new(value);
}
