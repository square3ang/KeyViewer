using KeyViewer.Models;
using UnityEngine;

namespace KeyViewer.Unity
{
    public class KeyManager : ADOBase
    {
        public Profile profile;

        private bool initialized;
        public void Init()
        {
            if (initialized) return;
            initialized = true;
        }
        void Update()
        {
            if (!initialized) return;
        }

        public static KeyManager CreateManager(string name, Profile profile)
        {
            if (profile == null) return null;
            GameObject manager = new GameObject(name ?? "KeyManager");
            DontDestroyOnLoad(manager);
            var km = manager.AddComponent<KeyManager>();
            km.profile = profile;
            return km;
        }
    }
}
