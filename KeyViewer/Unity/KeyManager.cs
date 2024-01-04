using KeyViewer.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer.Unity
{
    public class KeyManager : MonoBehaviour
    {
        public Profile profile;
        private Canvas keysCanvas;
        private RectTransform keysRt;
        private List<Key> keys;
        private bool initialized;
        public void Init()
        {
            if (initialized) return;
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(1280, 720);
            keys = new List<Key>();
            initialized = true;
        }
        public Key this[string dummyName]
        {
            get => keys.Find(k => k.config.DummyName.Equals(dummyName));
            set
            {
                int index = keys.FindIndex(k => k.config.DummyName.Equals(dummyName));
                if (index < 0) return;
                keys[index] = value;
            }
        }
        public Key this[KeyCode code]
        {
            get => keys.Find(k => k.config.Code.Equals(code));
            set
            {
                int index = keys.FindIndex(k => k.config.Code.Equals(code));
                if (index < 0) return;
                keys[index] = value;
            }
        }
        public Vector2 Position
        {
            get => keysRt.pivot;
            set
            {
                keysRt.anchorMin = value;
                keysRt.anchorMax = value;
                keysRt.pivot = value;
            }
        }
        public void UpdateKeys()
        {
            if (keysCanvas)
                Destroy(keysCanvas.gameObject);
            GameObject keysObject = new GameObject("Canvas");
            keysObject.transform.SetParent(transform);
            keysCanvas = keysObject.AddComponent<Canvas>();
            keysRt = keysCanvas.GetComponent<RectTransform>();
            keys = new List<Key>();
            foreach (KeyConfig config in profile.Keys)
            {
                string name = config.DummyName ?? config.Code.ToString();
                GameObject keyObject = new GameObject($"Key {name}");
                Key key = keyObject.AddComponent<Key>();
                key.Init(this, config);
                keys.Add(key);
            }
            UpdateLayout();
        }
        public void UpdateLayout()
        {
            int count = keys.Count;
            float keyHeight = profile.Keys.Any(k => k.EnableCountText) ? 150 : 100;
            float spacing = 10;
            float width = count * 100 + (count - 1) * spacing;

            var vecConfig = profile.VectorConfig;
            Position = vecConfig.Offset.Released;
            keysRt.sizeDelta = new Vector2(width, keyHeight);
            keysRt.anchoredPosition = Vector2.zero;
            keysRt.localRotation = Quaternion.Euler(vecConfig.Rotation.Released);
            keysRt.localScale = vecConfig.Scale.Released;

            float x = 0, tempX = 0;
            int updateCount = 0;
            keys.ForEach(k => k.UpdateLayout(ref x, ref tempX, updateCount++));
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
