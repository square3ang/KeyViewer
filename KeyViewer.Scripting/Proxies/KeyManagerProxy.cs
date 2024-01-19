using JSNet.API;
using KeyViewer.Models;
using KeyViewer.Unity;
using UnityEngine;

namespace KeyViewer.Scripting.Proxies
{
    [Alias("KeyManager")]
    public class KeyManagerProxy
    {
        [NotVisible]
        public KeyManager manager;
        public Profile Profile;
        public KeyManagerProxy(KeyManager manager)
        {
            this.manager = manager;
            Profile = manager.profile;
        }
        public Vector3 Rotation { get => manager.keysRt.localRotation.eulerAngles; set => manager.keysRt.localRotation = Quaternion.Euler(value); }
        public Vector3 Offset { get => manager.keysRt.localPosition; set => manager.keysRt.localPosition = value; }
        public Vector2 Scale { get => manager.keysRt.sizeDelta / manager.defaultSize; set => manager.keysRt.sizeDelta = manager.defaultSize * value; }
    }
}
