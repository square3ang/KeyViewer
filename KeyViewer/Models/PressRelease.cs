using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models;

public class PressRelease<T> : PressReleaseBase<T>, ICopyable<PressRelease<T>> {
    public EaseConfig PressedEase = new();
    public EaseConfig ReleasedEase = new();

    public PressRelease() { }
    public PressRelease(T value) : base(value) { }
    public PressRelease(T pressed, T released) : base(pressed, released) { }

    public EaseConfig GetEase(bool pressed = true)
        => pressed ? PressedEase : ReleasedEase;

    public PressRelease<T> SetEase(EaseConfig value) {
        PressedEase = value;
        ReleasedEase = value;
        return this;
    }

    public new PressRelease<T> Copy() {
        return new PressRelease<T> {
            Pressed = Pressed,
            Released = Released,
            PressedEase = PressedEase?.Copy(),
            ReleasedEase = ReleasedEase?.Copy()
        };
    }

    public override JToken Serialize() {
        var node = (JObject)base.Serialize();

        if(ReleasedEase?.IsValid == true) {
            node[nameof(ReleasedEase)] = ReleasedEase.Serialize();
        }

        if(PressedEase?.IsValid == true &&
           (PressedEase.Ease != ReleasedEase?.Ease ||
            PressedEase.Duration != ReleasedEase?.Duration)) {

            node[nameof(PressedEase)] = PressedEase.Serialize();
        }

        return node;
    }

    public override void Deserialize(JToken node) {
        base.Deserialize(node);

        if(node == null) {
            PressedEase = new EaseConfig();
            ReleasedEase = new EaseConfig();
            return;
        }

        var releasedEaseRaw = node[nameof(ReleasedEase)];
        var pressedEaseRaw = node[nameof(PressedEase)];

        ReleasedEase = releasedEaseRaw != null
            ? ModelUtils.Unbox<EaseConfig>(releasedEaseRaw)
            : new EaseConfig();

        PressedEase = pressedEaseRaw != null
            ? ModelUtils.Unbox<EaseConfig>(pressedEaseRaw)
            : ReleasedEase.Copy();
    }

    public static implicit operator PressRelease<T>(T value) => new(value);
}
