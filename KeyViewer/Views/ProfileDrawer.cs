using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TKP = KeyViewer.Core.Translation.TranslationKeys.Profile;
using TKM = KeyViewer.Core.Translation.TranslationKeys.Misc;
using TKKC = KeyViewer.Core.Translation.TranslationKeys.KeyConfig;
using KeyViewer.Controllers;

namespace KeyViewer.Views
{
    public class ProfileDrawer : ModelDrawable<Profile>
    {
        public KeyManager manager;
        public List<KeyConfigDrawer> drawers = new List<KeyConfigDrawer>();
        public List<KeyConfigDrawer> specialDrawers = new List<KeyConfigDrawer>();
        private bool listening = false;
        private bool configMode = false;
        public ProfileDrawer(KeyManager manager, Profile profile, string name) : base(profile, L(TKP.ConfigurateProfile, name))
        {
            this.manager = manager;
            drawers = profile.Keys.Where(kc => kc.SpecialKey == SpecialKeyType.None)
                .OrderBy(kc => kc.Code)
                .Select(kc => new KeyConfigDrawer(manager, kc))
                .ToList();
            specialDrawers = profile.Keys.Where(kc => kc.SpecialKey != SpecialKeyType.None)
                .OrderBy(kc => kc.SpecialKey)
                .Select(kc => new KeyConfigDrawer(manager, kc))
                .ToList();
        }
        public override void Draw()
        {
            Drawer.DrawBool(L(TKP.ViewOnlyGamePlay), ref model.ViewOnlyGamePlay);
            Drawer.DrawBool(L(TKP.AnimateKeys), ref model.AnimateKeys);
            Drawer.DrawBool(L(TKP.ShowKeyPressTotal), ref model.ShowKeyPressTotal);
            Drawer.DrawBool(L(TKP.LimitNotRegisteredKeys), ref model.LimitNotRegisteredKeys);
            Drawer.DrawBool(L(TKP.ResetOnStart), ref model.ResetOnStart);
            Drawer.DrawInt32(L(TKP.KPSUpdateRate), ref model.KPSUpdateRate);
            Drawer.DrawPressReleaseV(L(TKP.Scale), model.VectorConfig.Scale, Drawer.CD_V_VEC2_0_1_300);
            Drawer.DrawPressReleaseV(L(TKP.Offset), model.VectorConfig.Offset, Drawer.CD_V_VEC2_0_1_300);
            Drawer.DrawPressReleaseV(L(TKP.Rotation), model.VectorConfig.Rotation, Drawer.CD_V_VEC3_0_1_300);
            GUILayoutEx.HorizontalLine(1);
            GUILayout.Label(L(TKP.RegisteredKeys));
            DrawKeyConfigGUI();
        }
        public override void OnKeyDown(KeyCode code)
        {
            if (code == KeyCode.Mouse0) return;
            if (model.Keys.Any(kc => kc.Code == code))
            {
                model.Keys.RemoveAll(kc => kc.Code == code);
                drawers.RemoveAll(kcd => kcd.model.Code == code);
            }
            else
            {
                var config = new KeyConfig() { Code = code };
                model.Keys.Add(config);
                drawers.Add(new KeyConfigDrawer(manager, config));
            }
        }
        private void DrawKeyConfigGUI()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    for (int i = 0; i < model.Keys.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            var key = model.Keys[i];
                            var str = key.SpecialKey != SpecialKeyType.None ? key.SpecialKey.ToString() : key.Code.ToString();
                            if (GUILayout.Button(str))
                            {
                                if (configMode)
                                    GUIController.Push(new KeyConfigDrawer(manager, key));
                                else model.Keys.RemoveAt(i);
                                break;
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                    }    
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(!listening ? L(TKP.StartKeyRegistering) : L(TKP.StopKeyRegistering)))
                    {
                        if (Main.ListeningDrawer != null)
                            Main.ListeningDrawer = null;
                        else Main.ListeningDrawer = this;
                        listening = Main.ListeningDrawer != null;
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button(!configMode ? L(TKM.Enable, L(TKP.ConfigurationMode)) : L(TKM.Disable, L(TKP.ConfigurationMode))))
                        configMode = !configMode;
                    GUILayout.Space(10);
                    if (!model.Keys.Any(k => k.Code == KeyCode.Mouse0) && GUILayout.Button(L(TKP.RegisterMouse0Key)))
                        model.Keys.Add(new KeyConfig() { Code = KeyCode.Mouse0 });
                    GUILayout.Space(10);
                    if (!model.Keys.Any(k => k.SpecialKey == SpecialKeyType.KPS) && GUILayout.Button(L(TKP.RegisterKPSKey)))
                        model.Keys.Add(new KeyConfig() { SpecialKey = SpecialKeyType.KPS });
                    GUILayout.Space(10);
                    if (!model.Keys.Any(k => k.SpecialKey == SpecialKeyType.Total) && GUILayout.Button(L(TKP.RegisterTotalKey)))
                        model.Keys.Add(new KeyConfig() { SpecialKey = SpecialKeyType.Total });
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
