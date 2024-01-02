using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TKP = KeyViewer.Core.Translation.TranslationKeys.Profile;

namespace KeyViewer.Views
{
    public class ProfileDrawer : ModelDrawable<Profile>
    {
        public KeyManager manager;
        public List<KeyConfigDrawer> drawers = new List<KeyConfigDrawer>();
        public List<KeyConfigDrawer> specialDrawers = new List<KeyConfigDrawer>();
        private bool listening = false;
        public ProfileDrawer(KeyManager manager, Profile profile) : base(profile)
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
            Drawer.DrawBool(L(TKP.MakeBarSpecialKeys), ref model.MakeBarSpecialKeys);
            Drawer.DrawBool(L(TKP.ViewOnlyGamePlay), ref model.ViewOnlyGamePlay);
            Drawer.DrawBool(L(TKP.AnimateKeys), ref model.AnimateKeys);
            Drawer.DrawBool(L(TKP.ShowKeyPressTotal), ref model.ShowKeyPressTotal);
            Drawer.DrawBool(L(TKP.LimitNotRegisteredKeys), ref model.LimitNotRegisteredKeys);
            Drawer.DrawBool(L(TKP.ResetOnStart), ref model.ResetOnStart);
            Drawer.DrawInt32(L(TKP.KPSUpdateRate), ref model.KPSUpdateRate);
            
            Drawer.DrawVector2WithSlider(L(TKP.Size), ref model.VectorConfig.Scale);
            DrawKeyRegisterGUI();
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
        private void DrawKeyRegisterGUI()
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
                            if (GUILayout.Button(key.SpecialKey != SpecialKeyType.None ? key.SpecialKey.ToString() : key.Code.ToString()))
                            {
                                model.Keys.RemoveAt(i);
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
                    if (!model.Keys.Any(k => k.Code == KeyCode.Mouse0) && GUILayout.Button(L(TKP.RegisterMouse0Key)))
                    {
                        var kps = new KeyConfig() { Code = KeyCode.Mouse0 };
                        model.Keys.Add(kps);
                        drawers.Add(new KeyConfigDrawer(manager, kps));
                    }
                    if (!model.Keys.Any(k => k.SpecialKey == SpecialKeyType.KPS) && GUILayout.Button(L(TKP.RegisterKPSKey)))
                    {
                        var kps = new KeyConfig() { SpecialKey = SpecialKeyType.KPS };
                        model.Keys.Add(kps);
                        specialDrawers.Add(new KeyConfigDrawer(manager, kps));
                    }    
                    if (!model.Keys.Any(k => k.SpecialKey == SpecialKeyType.Total) && GUILayout.Button(L(TKP.RegisterTotalKey)))
                    {
                        var total = new KeyConfig() { SpecialKey = SpecialKeyType.Total };
                        model.Keys.Add(total);
                        specialDrawers.Add(new KeyConfigDrawer(manager, total));
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
