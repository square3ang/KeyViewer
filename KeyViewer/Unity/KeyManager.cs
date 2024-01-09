using KeyViewer.Core;
using KeyViewer.Core.TextReplacing;
using KeyViewer.Models;
using KeyViewer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer.Unity
{
    public class KeyManager : MonoBehaviour
    {
        public Tag CurKPSTag { get; private set; }
        public Tag MaxKPSTag { get; private set; }
        public Tag AvgKPSTag { get; private set; }
        public Tag CountTag { get; private set; }
        public Tag[] AllTags => new Tag[] { CurKPSTag, MaxKPSTag, AvgKPSTag, CountTag };

        public Profile profile;
        public Canvas keysCanvas;

        internal KPSCalculator kpsCalc;
        internal List<Key> keys;
        internal Vector2 centerOffset;
        private RectTransform keysRt;
        private bool initialized;
        private bool prevPressed;
        public void Init()
        {
            if (initialized) return;
            kpsCalc = new KPSCalculator(profile);
            kpsCalc.Start();
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(1280, 720);
            keys = new List<Key>();
            CurKPSTag = new Tag("CurKPS").SetGetter(new Func<string, int>(name =>
            {
                if (string.IsNullOrEmpty(name)) return kpsCalc.Kps;
                Key key = keys.Find(k => KeyViewerUtils.KeyName(k.Config) == name);
                if (key == null) return -1;
                if (!key.KpsCalc.Running) return 0;
                return key.KpsCalc.Kps;
            }));
            MaxKPSTag = new Tag("MaxKPS").SetGetter(new Func<string, int>(name =>
            {
                if (string.IsNullOrEmpty(name)) return kpsCalc.Max;
                Key key = keys.Find(k => KeyViewerUtils.KeyName(k.Config) == name);
                if (key == null) return -1;
                if (!key.KpsCalc.Running) return 0;
                return key.KpsCalc.Max;
            }));
            AvgKPSTag = new Tag("AvgKPS").SetGetter(new Func<string, double>(name =>
            {
                if (string.IsNullOrEmpty(name)) return kpsCalc.Average;
                Key key = keys.Find(k => KeyViewerUtils.KeyName(k.Config) == name);
                if (key == null) return -1;
                if (!key.KpsCalc.Running) return 0;
                return key.KpsCalc.Average;
            }));
            CountTag = new Tag("Count").SetGetter(new Func<string, int>(name =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    int total = 0;
                    foreach (var k in keys)
                        total += k.Config.Count;
                    return total;
                }
                Key key = keys.Find(k => KeyViewerUtils.KeyName(k.Config) == name);
                if (key == null) return -1;
                return key.Config.Count;
            }));
            initialized = true;
        }
        public Key this[string keyName]
        {
            get => keys.Find(k => KeyViewerUtils.KeyName(k.Config) == keyName);
            set
            {
                int index = keys.FindIndex(k => KeyViewerUtils.KeyName(k.Config) == keyName);
                if (index < 0) return;
                keys[index] = value;
            }
        }
        private void Update()
        {
            if (!initialized) return;
            var pressed = keys.Any(k => k.Pressed);
            if (prevPressed == pressed) return;
            prevPressed = pressed;
            KeyViewerUtils.ApplyVectorConfig(keysRt, profile.VectorConfig, pressed);
        }
        public void UpdateKeys()
        {
            if (keysCanvas)
                Destroy(keysCanvas.gameObject);
            GameObject keysObject = new GameObject("Keys Canvas");
            keysObject.transform.SetParent(transform);
            keysCanvas = keysObject.AddComponent<Canvas>();
            keysRt = keysCanvas.GetComponent<RectTransform>();
            keys = new List<Key>();
            foreach (KeyConfig config in profile.Keys)
            {
                string name = KeyViewerUtils.KeyName(config);
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
            keysRt.anchorMin = Vector2.zero;
            keysRt.anchorMax = Vector2.zero;
            keysRt.sizeDelta = new Vector2(width, keyHeight);
            keysRt.anchoredPosition = vecConfig.Offset.Released;
            keysRt.pivot = new Vector2(0.5f, 0.5f);
            keysRt.localRotation = Quaternion.Euler(vecConfig.Rotation.Released);
            keysRt.localScale = vecConfig.Scale.Released;

            float totalX = 0;
            foreach (Key k in keys)
                if (!k.Config.DisableSorting)
                    totalX += k.Config.VectorConfig.Scale.Released.x * 100 + 10;
            centerOffset = new Vector2(totalX / 2 - 5, keyHeight / 2);

            float x = 0;
            keys.ForEach(k => k.UpdateLayout(ref x));
        }
        public static KeyManager CreateManager(string name, Profile profile)
        {
            if (profile == null) return null;
            GameObject manager = new GameObject($"KeyViewer {name} Profile");
            DontDestroyOnLoad(manager);
            var km = manager.AddComponent<KeyManager>();
            km.profile = profile;
            return km;
        }
    }
}
