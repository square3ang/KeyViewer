using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models
{
    public class PressRelease<T> : IModel, ICopyable<PressRelease<T>>
    {
        public T Pressed;
        public T Released;
        public EaseConfig PressedEase = new EaseConfig();
        public EaseConfig ReleasedEase = new EaseConfig();
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
            return newPR;
        }
        public JToken Serialize() {
            var node = new JObject();
            if(Released != null) {
                node[nameof(Released)] = ModelUtils.ToNode<T>(Released);
            }
            if(Pressed != null && !IsSame) {
                node[nameof(Pressed)] = ModelUtils.ToNode<T>(Pressed);
            }
            if(ReleasedEase != null && ReleasedEase.IsValid) {
                node[nameof(ReleasedEase)] = ReleasedEase.Serialize();
            }
            if(PressedEase != null && PressedEase.IsValid && PressedEase != ReleasedEase) {
                node[nameof(PressedEase)] = PressedEase.Serialize();
            }
            return node;
        }
        public void Deserialize(JToken node)
        {
            JToken releasedRaw = node[nameof(Released)];
            JToken pressedRaw = node[nameof(Pressed)];
            bool nullReleased = releasedRaw == null;
            if(!nullReleased) {
                Released = (T)ModelUtils.ToObject<T>(releasedRaw);
            }
            if(pressedRaw == null) {
                if(nullReleased) {
                    Pressed = default;
                } else {
                    Pressed = Released;
                }
            } else {
                Pressed = (T)ModelUtils.ToObject<T>(pressedRaw);
            }
            JToken releasedEaseRaw = node[nameof(ReleasedEase)];
            JToken pressedEaseRaw = node[nameof(PressedEase)];
            bool nullReleasedEase = releasedEaseRaw == null;
            if(!nullReleasedEase) {
                ReleasedEase = ModelUtils.Unbox<EaseConfig>(releasedEaseRaw);
            }
            if(pressedEaseRaw == null) {
                if(nullReleasedEase) {
                    PressedEase = new EaseConfig();
                } else {
                    PressedEase = ReleasedEase;
                }
            } else {
                PressedEase = ModelUtils.Unbox<EaseConfig>(pressedEaseRaw);
            }
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
        public new JToken Serialize()
        {
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
        public new void Deserialize(JToken node)
        {
            JToken releasedRaw = node[nameof(Released)];
            JToken pressedRaw = node[nameof(Pressed)];
            bool nullReleased = releasedRaw == null;
            if(!nullReleased) {
                Released = ModelUtils.Unbox<T>(node[nameof(Released)]);
            }
            if(pressedRaw == null) {
                if(nullReleased) {
                    Pressed = default;
                } else {
                    Pressed = Released;
                }
            } else {
                Pressed = ModelUtils.Unbox<T>(node[nameof(Pressed)]);
            }
            JToken releasedEaseRaw = node[nameof(ReleasedEase)];
            JToken pressedEaseRaw = node[nameof(PressedEase)];
            bool nullReleasedEase = releasedEaseRaw == null;
            if(!nullReleasedEase) {
                ReleasedEase = ModelUtils.Unbox<EaseConfig>(releasedRaw);
            }
            if(pressedEaseRaw == null) {
                if(nullReleasedEase) {
                    PressedEase = new EaseConfig();
                } else {
                    PressedEase = ReleasedEase;
                }
            } else {
                PressedEase = ModelUtils.Unbox<EaseConfig>(pressedRaw);
            }
        }
        public new PressReleaseM<T> Copy()
        {
            var newPR = new PressReleaseM<T>();
            newPR.Pressed = Pressed.Copy();
            newPR.Released = Released.Copy();
            newPR.PressedEase = PressedEase.Copy();
            newPR.ReleasedEase = ReleasedEase.Copy();
            return newPR;
        }
        public static implicit operator PressReleaseM<T>(T value) => new PressReleaseM<T>(value);
    }
}
