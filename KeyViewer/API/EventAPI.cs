using System;

namespace KeyViewer.API
{
    public static class EventAPI
    {
        public static event Action<KeyManager> OnUpdateKeysLayout = delegate { };
        public static event Action<Key> OnUpdateKeyLayout = delegate { };
        public static event Action OnClearCounts = delegate { };
        internal static void UpdateLayout(KeyManager manager) => OnUpdateKeysLayout(manager);
        internal static void UpdateLayout(Key key) => OnUpdateKeyLayout(key);
        internal static void ClearCounts() => OnClearCounts();
    }
}
