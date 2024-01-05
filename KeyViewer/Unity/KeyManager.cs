﻿using KeyViewer.Core;
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
        private RectTransform keysRt;
        private List<Key> keys;
        private bool initialized;
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
                Key key = keys.Find(k => KeyViewerUtils.KeyName(k.config) == name);
                if (key == null) return -1;
                if (!key.kpsCalc.Running) return 0;
                return key.kpsCalc.Kps;
            }));
            MaxKPSTag = new Tag("MaxKPS").SetGetter(new Func<string, int>(name =>
            {
                if (string.IsNullOrEmpty(name)) return kpsCalc.Max;
                Key key = keys.Find(k => KeyViewerUtils.KeyName(k.config) == name);
                if (key == null) return -1;
                if (!key.kpsCalc.Running) return 0;
                return key.kpsCalc.Max;
            }));
            AvgKPSTag = new Tag("AvgKPS").SetGetter(new Func<string, double>(name =>
            {
                if (string.IsNullOrEmpty(name)) return kpsCalc.Average;
                Key key = keys.Find(k => KeyViewerUtils.KeyName(k.config) == name);
                if (key == null) return -1;
                if (!key.kpsCalc.Running) return 0;
                return key.kpsCalc.Average;
            }));
            CountTag = new Tag("Count").SetGetter(new Func<string, int>(name =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    int total = 0;
                    foreach (var k in keys)
                        total += k.config.Count;
                    return total;
                }
                Key key = keys.Find(k => KeyViewerUtils.KeyName(k.config) == name);
                if (key == null) return -1;
                if (!key.kpsCalc.Running) return 0;
                return key.config.Count;
            }));
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

            float x = 0;
            keys.ForEach(k => k.UpdateLayout(ref x));
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
