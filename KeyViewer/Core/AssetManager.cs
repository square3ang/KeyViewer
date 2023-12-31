using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KeyViewer.Core
{
    public static class AssetManager
    {
        public static Sprite Background { get; private set; }
        public static Sprite Outline { get; private set; }
        private static Dictionary<string, Sprite> others;
        public static void Initialize()
        {
            Texture2D t = new Texture2D(1, 1);
            t.LoadImage(File.ReadAllBytes(Path.Combine(Main.Mod.Path, "Assets", "KeyBackground.png")));
            Background = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(.5f, .5f));
            t.LoadImage(File.ReadAllBytes(Path.Combine(Main.Mod.Path, "Assets", "KeyOutline.png")));
            Outline = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(.5f, .5f));
            others = new Dictionary<string, Sprite>();
        }
        public static void Release()
        {
            Object.Destroy(Background);
            Object.Destroy(Outline);
            foreach (var spr in others.Values)
                Object.Destroy(spr);
            Background = null;
            Outline = null;
            others = null;
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
