using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
        if(!EqualityComparer<T>.Default.Equals(Released, default)) {
            node[nameof(Released)] = ModelUtils.ToNode<T>(Released);
        }
        if(!EqualityComparer<T>.Default.Equals(Pressed, default) && !IsSame) {
            node[nameof(Pressed)] = ModelUtils.ToNode<T>(Pressed);
        }
        return node;
    }
    public virtual void Deserialize(JToken node) {
        if(node == null) {
            Pressed = default;
            Released = default;
            return;
        }
        var releasedRaw = node[nameof(Released)];
        var pressedRaw = node[nameof(Pressed)];
        bool hasReleased = releasedRaw != null;
        if(hasReleased) {
            Released = (T)ModelUtils.ToObject<T>(releasedRaw);
        }
        Pressed = pressedRaw != null ? (T)ModelUtils.ToObject<T>(pressedRaw) : hasReleased ? Released : default;
    }

    public bool IsSame => EqualityComparer<T>.Default.Equals(Pressed, Released);

    public static implicit operator PressReleaseBase<T>(T value) => new(value);
}
