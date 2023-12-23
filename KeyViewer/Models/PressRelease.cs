namespace KeyViewer.Models
{
    public class PressRelease<T>
    {
        public bool Enabled;
        public bool Expanded;
        public T Pressed;
        public T Released;
        public PressRelease() { }
        public PressRelease(T value) => Set(value);
        public PressRelease(T pressed, T released)
        {
            Pressed = pressed;
            Released = released;
            Enabled = true;
        }
        public T Get(bool pressed = true) => Enabled ? (pressed ? Pressed : Released) : Pressed;
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
        public bool IsSame => Equals(Pressed, Released);
        public static implicit operator PressRelease<T>(T value) => new PressRelease<T>(value);
    }
}
