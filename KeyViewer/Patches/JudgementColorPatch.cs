using HarmonyLib;
using KeyViewer.Core.Input;
using KeyViewer.Unity;
using KeyViewer.Utils;
using SkyHook;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Patches
{
    [HarmonyPatch]
    public static class JudgementColorPatch
    {
        static bool initialized = false;
        static HashSet<Key> keys = new HashSet<Key>();
        public static void Initialize()
        {
            if (initialized) return;
            SkyHookManager.KeyUpdated.AddListener(HookEvent);
            initialized = true;
        }
        public static void Release()
        {
            if (!initialized) return;
            SkyHookManager.KeyUpdated.RemoveListener(HookEvent);
            initialized = false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "ValidInputWasTriggered")]
        public static void SyncInput_StackPushPatch(scrController __instance)
        {
            if (!initialized) return;
            if (AsyncInputManager.isActive) return;
            foreach (var manager in Main.Managers.Values)
                foreach (var key in manager.keys)
                    if (Input.GetKeyDown(key.Config.Code))
                        keys.Add(key);
        }
        static bool stackFlushed = false;
        static States prevState = States.None;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "Update")]
        public static void StackFlushPatch(scrController __instance)
        {
            if (!initialized) return;
            var state = __instance.state;
            if (!stackFlushed && state == States.PlayerControl)
            {
                keys.Clear();
                AsyncInputManager.ClearKeys();
                stackFlushed = true;
            }
            prevState = state;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
        public static void StackFlushPatch2(scrController __instance)
        {
            if (!initialized) return;
            AsyncInputManager.ClearKeys();
            foreach (var manager in Main.Managers.Values)
                if (manager.profile.ResetOnStart)
                    foreach (var key in manager.keys)
                        key.Config.Count = 0;
            stackFlushed = false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrMistakesManager), "AddHit")]
        public static void AddHitPatch(HitMargin hit)
        {
            if (!initialized) return;
            foreach (var key in keys)
            {
                var textConfig = key.Config.TextConfig;
                if (textConfig.ChangeColorWithJudge)
                {
                    key.IgnoreColorUpdate(Key.Element.Text);
                    var judge = textConfig.JudgeColors;
                    KeyViewerUtils.ApplyColor(key.Text, textConfig.Color.Released, judge.Get(hit), textConfig.JudgeColorEase);
                }

                if (key.Config.EnableCountText)
                {
                    var countTextConfig = key.Config.CountTextConfig;
                    if (countTextConfig.ChangeColorWithJudge)
                    {
                        key.IgnoreColorUpdate(Key.Element.CountText);
                        var judge = countTextConfig.JudgeColors;
                        KeyViewerUtils.ApplyColor(key.CountText, countTextConfig.Color.Released, judge.Get(hit), countTextConfig.JudgeColorEase);
                    }
                }

                var bgConfig = key.Config.BackgroundConfig;
                if (bgConfig.ChangeColorWithJudge)
                {
                    key.IgnoreColorUpdate(Key.Element.Background);
                    var judge = bgConfig.JudgeColors;
                    KeyViewerUtils.ApplyColor(key.Background, bgConfig.Color.Released, judge.Get(hit), bgConfig.JudgeColorEase);
                }

                if (key.Config.EnableOutlineImage)
                {
                    var olConfig = key.Config.OutlineConfig;
                    if (olConfig.ChangeColorWithJudge)
                    {
                        key.IgnoreColorUpdate(Key.Element.Outline);
                        var judge = olConfig.JudgeColors;
                        KeyViewerUtils.ApplyColor(key.Outline, olConfig.Color.Released, judge.Get(hit), olConfig.JudgeColorEase);
                    }
                }

                if (key.Config.RainEnabled)
                {
                    var rainConfig = key.Config.Rain.ObjectConfig;
                    if (rainConfig.ChangeColorWithJudge)
                    {
                        key.rain.IgnoreColorUpdate();
                        var judge = rainConfig.JudgeColors;
                        KeyViewerUtils.ApplyColor(key.rain.image, rainConfig.Color.Released, judge.Get(hit), rainConfig.JudgeColorEase);
                    }
                }
            }
            keys.Clear();
        }
        private static void HookEvent(SkyHookEvent she)
        {
            try
            {
                if (!AsyncInputManager.isActive) return;
                if (she.Type == SkyHook.EventType.KeyReleased) return;
                if (Main.IsEnabled && (scrController.instance?.gameworld ?? false))
                {
                    var code = AsyncInputCompat.Convert(she.Label);
                    foreach (var manager in Main.Managers.Values)
                        foreach (var key in manager.keys)
                            if (key.Config.Code == code)
                                keys.Add(key);
                }
            }
            catch { }
        }
    }
}
