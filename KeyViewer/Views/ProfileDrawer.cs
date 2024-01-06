using KeyViewer.Controllers;
using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using System.Linq;
using UnityEngine;
using TKM = KeyViewer.Core.Translation.TranslationKeys.Misc;
using TKP = KeyViewer.Core.Translation.TranslationKeys.Profile;

namespace KeyViewer.Views
{
    public class ProfileDrawer : ModelDrawable<Profile>
    {
        public KeyManager manager;
        private bool listening = false;
        private bool configMode = true;
        private int dummyNumber = 1;
        public ProfileDrawer(KeyManager manager, Profile profile, string name) : base(profile, L(TKP.ConfigurateProfile, name))
        {
            this.manager = manager;
        }
        public override void Draw()
        {
            Drawer.DrawBool(L(TKP.ViewOnlyGamePlay), ref model.ViewOnlyGamePlay);
            Drawer.DrawBool(L(TKP.AnimateKeys), ref model.AnimateKeys);
            Drawer.DrawBool(L(TKP.LimitNotRegisteredKeys), ref model.LimitNotRegisteredKeys);
            Drawer.DrawBool(L(TKP.ResetOnStart), ref model.ResetOnStart);
            Drawer.DrawInt32(L(TKP.KPSUpdateRate), ref model.KPSUpdateRate);
            Drawer.DrawVectorConfig(model.VectorConfig);
            GUILayoutEx.HorizontalLine(1);
            GUILayout.Label(L(TKP.RegisteredKeys));
            DrawKeyConfigGUI();
        }
        public override void OnKeyDown(KeyCode code)
        {
            if (code == KeyCode.Mouse0) return;
            if (model.Keys.Any(kc => kc.Code == code))
                model.Keys.RemoveAll(kc => kc.Code == code);
            else model.Keys.Add(new KeyConfig() { Code = code });
            manager.UpdateKeys();
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
                            var str = key.DummyName != null ? key.DummyName : key.Code.ToString();
                            if (GUILayout.Button(str))
                            {
                                if (configMode)
                                    GUIController.Push(new KeyConfigDrawer(manager, key));
                                else
                                {
                                    model.Keys.RemoveAt(i);
                                    manager.UpdateKeys();
                                }
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
                    if (GUILayout.Button(L(TKP.CreateDummyKey)))
                    {
                        var dummy = new KeyConfig() { DummyName = L(TKP.DummyName, dummyNumber++) };
                        model.Keys.Add(dummy);
                        manager.UpdateKeys();
                    }
                    GUILayout.Space(10);
                    if (!model.Keys.Any(k => k.Code == KeyCode.Mouse0) && GUILayout.Button(L(TKP.RegisterMouse0Key)))
                    {
                        model.Keys.Add(new KeyConfig() { Code = KeyCode.Mouse0 });
                        manager.UpdateKeys();
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
