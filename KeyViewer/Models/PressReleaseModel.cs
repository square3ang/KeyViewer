using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models;

public class PressReleaseModel<T> : PressRelease<T>, ICopyable<PressReleaseModel<T>>
where T : IModel, ICopyable<T>, new() {
    public PressReleaseModel() { }
    public PressReleaseModel(T value) : base(value) { }
    public PressReleaseModel(T pressed, T released) : base(pressed, released) { }
    public override JToken Serialize() {
        var node = new JObject();
        if(Released != null) {
            node[nameof(Released)] = Released.Serialize();
        }
        if(Pressed != null && !IsSame) {
            node[nameof(Pressed)] = Pressed.Serialize();
        }

        if(ReleasedEase != null && ReleasedEase.IsValid) {
            node[nameof(ReleasedEase)] = ReleasedEase.Serialize();
        }

        if(PressedEase != null &&
           PressedEase.IsValid &&
           (PressedEase.Ease != ReleasedEase?.Ease ||
            PressedEase.Duration != ReleasedEase?.Duration)) {

            node[nameof(PressedEase)] = PressedEase.Serialize();
        }
        return node;
    }
    public new PressReleaseModel<T> Copy() {
        T pressedCopy = Pressed != null ? Pressed.Copy() : default;
        T releasedCopy = Released != null ? Released.Copy() : default;
        return new PressReleaseModel<T>(pressedCopy, releasedCopy) {
            PressedEase = PressedEase?.Copy() ?? new EaseConfig(),
            ReleasedEase = ReleasedEase?.Copy() ?? new EaseConfig()
        };
    }
    public override void Deserialize(JToken node) {
        if(node == null) {
            Pressed = default;
            Released = default;
            PressedEase = new EaseConfig();
            ReleasedEase = new EaseConfig();
            return;
        }
        var releasedRaw = node[nameof(Released)];
        var pressedRaw = node[nameof(Pressed)];
        bool hasReleased = releasedRaw != null;
        if(hasReleased) {
            Released = ModelUtils.Unbox<T>(releasedRaw);
        }
        Pressed = pressedRaw != null
            ? ModelUtils.Unbox<T>(pressedRaw)
            : hasReleased ? Released.Copy() : default;
        var releasedEaseRaw = node[nameof(ReleasedEase)];
        var pressedEaseRaw = node[nameof(PressedEase)];
        ReleasedEase = releasedEaseRaw != null
            ? ModelUtils.Unbox<EaseConfig>(releasedEaseRaw)
            : new EaseConfig();
        PressedEase = pressedEaseRaw != null
            ? ModelUtils.Unbox<EaseConfig>(pressedEaseRaw)
            : ReleasedEase.Copy();
    }
}
