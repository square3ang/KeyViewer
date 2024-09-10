using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Core.Translation;
using KeyViewer.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Controllers
{
    public class GUIController
    {
        private List<IDrawable> drawables = new List<IDrawable>();
        private int depth;
        private bool isUndoAvailable => depth > 0;
        private bool isRedoAvailable => depth < drawables.Count;
        private IDrawable current;
        private IDrawable first;
        private int skipFrames = 0;
        private Stack<Action> onSkipCallbacks = new Stack<Action>();
        public void Init(IDrawable drawable)
        {
            first = current = drawable;
        }
        public void Push(IDrawable drawable)
        {
            if (drawables.Count == depth)
            {
                drawables.Add(current);
                depth++;
            }
            else
            {
                if (drawable.Name != drawables[depth].Name)
                {
                    drawables.RemoveRange(depth, drawables.Count - depth);
                    drawables.Add(current);
                    depth++;
                }
                else drawables[depth++] = current;
            }
            current = drawable;
        }
        public void Pop()
        {
            if (!isUndoAvailable) return;
            var cache = current;
            current = drawables[--depth];
            drawables[depth] = cache;
        }
        public void Draw()
        {
            if (skipFrames > 0)
            {
                skipFrames--;
                if (onSkipCallbacks.Count > 0)
                    onSkipCallbacks.Pop()?.Invoke();
                return;
            }
            GUILayout.BeginHorizontal();
            {
                if (isUndoAvailable)
                {
                    if (Drawer.Button("◀ " + drawables[depth - 1].Name))
                        Pop();
                }
                if (isRedoAvailable)
                {
                    var draw = drawables[depth];
                    if (Drawer.Button(draw.Name + " ▶"))
                        Push(draw);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            current.Draw();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Main.Lang[TranslationKeys.KO], GUI.skin.label)) KeyViewerUtils.OpenMysteryUrl();
            if (GUILayout.Button(Main.Lang[TranslationKeys.LINK], GUI.skin.label)) KeyViewerUtils.OpenDiscord2Url();
            GUILayout.Space(1);
            if (GUILayout.Button(Main.Lang[TranslationKeys.KW], GUI.skin.label)) KeyViewerUtils.OpenMysteryUrl();
            if (GUILayout.Button(Main.Lang[TranslationKeys.LINK], GUI.skin.label)) KeyViewerUtils.OpenWikiUrl();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            string[] ver = Main.Lang[TranslationKeys.Misc.RealLatestVersion].Split('.');
            if (GUILayout.Button($"<color=#2BDCFF>C</color><color=#33DDF5>#</color><color=#3BDEEC>#</color><color=#44E0E3>'</color><color=#4CE1D9>s</color> <color=#5DE4C7>M</color><color=#66E5BE>O</color><color=#6EE7B4>D</color> <color=#7FEAA2>S</color><color=#88EB98>e</color><color=#90EC8F>r</color><color=#99EE86>v</color><color=#A1EF7D>e</color><color=#AAF173>r</color><color=#B2F26A>!</color> <color=#C3F557>(</color><color=#CCF64E>K</color><color=#D4F845>e</color><color=#DDF93C>y</color><color=#E5FA32>V</color><color=#EEFC29>i</color><color=#F6FD20>e</color><color=#FEFF16>w</color><color=#FEF81F>e</color><color=#FEF127>r</color> <color=#FEE338>L</color><color=#FEDC40>a</color><color=#FED549>t</color><color=#FECF51>e</color><color=#FEC859>s</color><color=#FEC162>t</color> <color=#FEB372>V</color><color=#FEAC7B>e</color><color=#FEA683>r</color><color=#FE9F8C>s</color><color=#FE9894>i</color><color=#FE919C>o</color><color=#FE8AA5>n</color><color=#FE83AD>:</color><color=#FE7DB5>{ver[0]}</color><color=#FE76BE>.</color><color=#FE6FC6>{ver[1]}</color><color=#FE68CE>.</color><color=#FE61D7>{ver[2]}</color><color=#FE5ADF>)</color>", GUI.skin.label))
                Application.OpenURL(Main.DiscordLink);

            if (GUILayout.Button(Main.Lang[TranslationKeys.BOATK], GUI.skin.label))
                Application.OpenURL("https://github.com/PizzaLovers007/AdofaiTweaks/tree/master/AdofaiTweaks/Tweaks/KeyViewer");
        }
        public void Skip(Action onSkip = null, int frames = 1)
        {
            skipFrames += frames;
            onSkipCallbacks.Push(onSkip);
        }
        public void Flush()
        {
            current = first;
            drawables = new List<IDrawable>();
            depth = 0;
            onSkipCallbacks = new Stack<Action>();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, false);
        }
    }
}
