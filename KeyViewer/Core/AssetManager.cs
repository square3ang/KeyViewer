using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KeyViewer.Core
{
    public static class AssetManager
    {
        public static bool Initialized { get; private set; }
        public static Sprite Background { get; private set; }
        public static Sprite Outline { get; private set; }
        public static Shader RoundedCorners { get; private set; }
        public static Shader IndependentRoundedCorners { get; private set; }
        private static Dictionary<string, Sprite> others;
        public static void Initialize()
        {
            if (Initialized) return;
            var request = AssetBundle.LoadFromFileAsync(Path.Combine(Main.Mod.Path, "KeyViewer.assets"));
            request.completed += o =>
            {
                var assets = request.assetBundle;
                Background = assets.LoadAsset<Sprite>("Assets/Images/KeyBackground.png");
                Outline = assets.LoadAsset<Sprite>("Assets/Images/KeyOutline.png");
                RoundedCorners = assets.LoadAsset<Shader>("Assets/Shaders/RoundedCorners.shader");
                IndependentRoundedCorners = assets.LoadAsset<Shader>("Assets/Shaders/IndependentRoundedCorners.shader");
                others = new Dictionary<string, Sprite>();
                Main.Logger.Log($"Loaded Key Viewer's Assets");
                Initialized = true;
            };
        }
        public static void Release()
        {
            if (!Initialized) return;
            Object.Destroy(Background);
            Object.Destroy(Outline);
            Object.Destroy(RoundedCorners);
            Object.Destroy(IndependentRoundedCorners);
            foreach (var spr in others.Values)
                Object.Destroy(spr);
            Background = null;
            Outline = null;
            others = null;
            Initialized = false;
        }
        public static Sprite Get(string path)
        {
            if (others.TryGetValue(path, out var spr))
                return spr;
            if (!File.Exists(path)) return null;
            Texture2D t = new Texture2D(1, 1);
            t.LoadImage(File.ReadAllBytes(path));
            return others[path] = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(.5f, .5f));
        }
    }
}
