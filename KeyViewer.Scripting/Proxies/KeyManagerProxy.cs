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
        public Vector2 Offset { get => manager.keysRt.anchoredPosition; set => manager.keysRt.anchoredPosition = value; }
        public Vector2 Scale { get => manager.keysRt.localScale; set => manager.keysRt.localScale = value; }
    }
}
