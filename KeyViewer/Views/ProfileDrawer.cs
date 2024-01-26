using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using System.Collections.Generic;
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
        private HashSet<KeyConfig> selectedKeys = new HashSet<KeyConfig>();
        private KeyConfig criterion;
        public ProfileDrawer(KeyManager manager, Profile profile, string name) : base(profile, L(TKP.ConfigurateProfile, name))
        {
            this.manager = manager;
        }
        public override void Draw()
        {
            bool changed = false;
            Drawer.DrawBool(L(TKP.ViewOnlyGamePlay), ref model.ViewOnlyGamePlay);
            changed |= Drawer.DrawBool(L(TKP.LimitNotRegisteredKeys), ref model.LimitNotRegisteredKeys);
            changed |= Drawer.DrawBool(L(TKP.ResetOnStart), ref model.ResetOnStart);
            changed |= Drawer.DrawInt32(L(TKP.KPSUpdateRate), ref model.KPSUpdateRate);
            changed |= Drawer.DrawSingleWithSlider(L(TKP.KeySpacing), ref model.KeySpacing, 0, 100, 300f);
            changed |= Drawer.DrawVectorConfig(model.VectorConfig);
            GUILayoutEx.HorizontalLine(1);
            DrawKeyConfigGUI();
            if (changed) manager.UpdateLayout();
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
            GUILayout.BeginHorizontal();
            {
                Drawer.ButtonLabel(L(TKP.RegisteredKeys), KeyViewerUtils.OpenDiscordUrl);
                if (model.Keys.Any(k => !selectedKeys.Contains(k)))
                {
                    if (GUILayout.Button(L(TKP.SelectAllKeys)))
                        model.Keys.ForEach(k => selectedKeys.Add(k));
                }
                else
                {
                    if (GUILayout.Button(L(TKP.DeselectAllKeys)))
                    {
                        selectedKeys.Clear();
                        criterion = null;
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

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
                            var selected = selectedKeys.Contains(key);
                            if (criterion == key) str = $"<color=yellow>{str}</color>";
                            else if (selected) str = $"<color=cyan>{str}</color>";
                            if (GUILayout.Button(str))
                            {
                                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                                {
                                    if (!selectedKeys.Add(key))
                                    {
                                        if (criterion != key)
                                            criterion = key;
                                        else
                                        {
                                            selectedKeys.Remove(key);
                                            criterion = null;
                                        }
                                    }
                                }
                                else if (configMode)
                                    Main.GUI.Push(new KeyConfigDrawer(manager, key));
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
                    if (!model.Keys.Any(k => k.Code == KeyCode.Mouse0))
                    {
                        GUILayout.Space(10);
                        if (GUILayout.Button(L(TKP.RegisterMouse0Key)))
                        {
                            model.Keys.Add(new KeyConfig() { Code = KeyCode.Mouse0 });
                            manager.UpdateKeys();
                        }
                    }
                    if (selectedKeys.Count > 0)
                    {
                        GUILayout.Space(10);
                        if (GUILayout.Button(L(TKP.MakeBar)))
                        {
                            KeyViewerUtils.MakeBar(manager.profile, manager.keys.FindAll(k => selectedKeys.Contains(k.Config)).Select(k => k.Config).ToList());
                            manager.UpdateLayout();
                            selectedKeys.Clear();
                            criterion = null;
                        }
                    }
                    if (selectedKeys.Count > 1)
                    {
                        GUILayout.Space(10);
                        if (GUILayout.Button(L(TKP.EditMultipleKey)))
                        {
                            Main.GUI.Push(new MultipleKeyConfigDrawer(manager, selectedKeys.Select(k => KeyViewerUtils.KeyName(k)).ToList(), criterion.Copy()));
                            selectedKeys.Clear();
                            criterion = null;
                        }
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
