using KeyViewer.Models;
using UnityEngine;

namespace KeyViewer.Unity
{
    public class KeyManager : ADOBase
    {
        public Profile profile;

        private bool initialized;
        public void Init(Profile profile)
        {
            this.profile = profile;
        }
        void Update()
        {
            if (!initialized) return;
        }

        public static KeyManager CreateManager(string name, Profile profile)
        {
            if (profile == null) return null;
            GameObject manager = new GameObject(name ?? "KeyManager");
            KeyManager keyManager = manager.AddComponent<KeyManager>();
            keyManager.Init(profile);
            return keyManager;
        }
    }
}
