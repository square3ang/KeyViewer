using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models {
    public class PressReleaseModel<T>
    : PressRelease<T>, ICopyable<PressReleaseModel<T>>
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
            if(ReleasedEase != null && PressedEase.IsValid) {
                node[nameof(ReleasedEase)] = ReleasedEase.Serialize();
            }
            if(PressedEase != null && (PressedEase.IsValid && PressedEase != ReleasedEase || !ReleasedEase.IsValid)) {
                node[nameof(PressedEase)] = PressedEase.Serialize();
            }
            return node;
        }

        public new PressReleaseModel<T> Copy() {
            T pressedCopy = Pressed != null ? Pressed.Copy() : default;
            T releasedCopy = Released != null ? Released.Copy() : default;
            var copy = new PressReleaseModel<T>(pressedCopy, releasedCopy);
            copy.PressedEase = PressedEase != null ? PressedEase.Copy() : new EaseConfig();
            copy.ReleasedEase = ReleasedEase != null ? ReleasedEase.Copy() : new EaseConfig();
            return copy;
        }

        public override void Deserialize(JToken node) {
            JToken releasedRaw = node[nameof(Released)];
            JToken pressedRaw = node[nameof(Pressed)];

            bool nullReleased = releasedRaw == null;

            if(!nullReleased) {
                Released = ModelUtils.Unbox<T>(releasedRaw);
            }

            if(pressedRaw == null) {
                Pressed = nullReleased ? default : Released;
            } else {
                Pressed = ModelUtils.Unbox<T>(pressedRaw);
            }

            JToken releasedEaseRaw = node[nameof(ReleasedEase)];
            JToken pressedEaseRaw = node[nameof(PressedEase)];

            bool nullReleasedEase = releasedEaseRaw == null;

            if(!nullReleasedEase) {
                ReleasedEase = ModelUtils.Unbox<EaseConfig>(releasedEaseRaw);
            } else {
                ReleasedEase = new EaseConfig();
            }

            if(pressedEaseRaw == null) {
                PressedEase = nullReleasedEase ? new EaseConfig() : ReleasedEase;
            } else {
                PressedEase = ModelUtils.Unbox<EaseConfig>(pressedEaseRaw);
            }
        }
    }
}
