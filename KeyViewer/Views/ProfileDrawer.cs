using KeyViewer.Core;
using KeyViewer.Core.Input;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using Overlayer.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeyViewer.Views;

public class ProfileDrawer : ModelDrawable<Profile> {
    public KeyManager manager;
    private bool listening = false;
    private bool configMode = true;
    private int dummyNumber = 1;
    private HashSet<KeyConfig> selectedKeys = [];
    private KeyConfig criterion;
    public ProfileDrawer(KeyManager manager, Profile profile, string name) : base(profile, string.Format(Main.Lang.Get("CONFIGURATE_PROFILE", "Configurate {0} Profile"), name)) => this.manager = manager;
    public override void OnceCall() => NeoDrawer.StaticInstance.FieldResetDictById();
    public override void Draw() {
        NeoDrawer.StaticInstance.FieldResetDictById();

        bool changed = false;
        DrawKeyConfigGUI();
        GUILayoutEx.HorizontalLine(1);
        Drawer.DrawBool(Main.Lang.Get("VIEW_ONLY_GAME_PLAY", "View Only Game Play"), ref model.ViewOnlyGamePlay);
        changed |= Drawer.DrawBool(Main.Lang.Get("LIMIT_NOT_REGISTERED_KEYS", "Limit Input Not Registered Keys"), ref model.LimitNotRegisteredKeys);
        changed |= Drawer.DrawBool(Main.Lang.Get("RESET_ON_START", "Reset On Start"), ref model.ResetOnStart);
        changed |= NeoDrawer.StaticInstance.DrawInt32(Main.Lang.Get("KPS_UPDATE_RATE", "KPS Update Rate"), ref model.KPSUpdateRate);
        changed |= NeoDrawer.StaticInstance.DrawSingleWithSlider(Main.Lang.Get("KEY_SPACING", "Key Spacing"), ref model.KeySpacing, 0, 100, 300f);
        changed |= NeoDrawer.StaticInstance.DrawVectorConfig(model.VectorConfig);
        if(changed) {
            manager.UpdateLayout();
        }

        NeoDrawer.StaticInstance.UpdateFocused();
    }
    public override void OnKeyDown(KeyCode code) {
        if(code == KeyCode.Mouse0) {
            return;
        }

        if(model.Keys.Any(kc => kc.Code == code)) {
            model.Keys.RemoveAll(kc => kc.Code == code);
        } else {
            model.Keys.Add(new KeyConfig() { Code = code });
        }

        manager.UpdateKeys();
    }
    private void DrawKeyConfigGUI() {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(Main.Lang.Get("REGISTERED_KEYS", "Registered Keys"));
            if(model.Keys.Any(k => !selectedKeys.Contains(k))) {
                if(Drawer.Button(Main.Lang.Get("SELECT_ALL_KEYS", "Select All Keys"))) {
                    model.Keys.ForEach(k => selectedKeys.Add(k));
                }
            } else {
                if(Drawer.Button(Main.Lang.Get("DESELECT_ALL_KEYS", "Deselect All Keys"))) {
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
                for(int i = 0; i < model.Keys.Count; i++) {
                    GUILayout.BeginHorizontal();
                    {
                        var key = model.Keys[i];
                        var str = key.DummyName ?? key.Code.ToString();

                        var selected = selectedKeys.Contains(key);
                        if(criterion == key) {
                            str = $"<color=yellow>{str}</color>";
                        } else if(selected) {
                            str = $"<color=cyan>{str}</color>";
                        }

                        if(Drawer.Button(str)) {
                            if(KeyInput.Shift) {
                                if(!selectedKeys.Add(key)) {
                                    if(criterion != key)
                                        criterion = key;
                                    else {
                                        selectedKeys.Remove(key);
                                        criterion = null;
                                    }
                                }
                            } else if(configMode)
                                Main.GUI.Push(new KeyConfigDrawer(manager, key));
                            else {
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
                if(Drawer.Button(!listening ? Main.Lang.Get("START_KEY_REGISTERING", "Start Key Register") : Main.Lang.Get("STOP_KEY_REGISTERING", "Stop Key Register"))) {
                    Main.ListeningDrawer = Main.ListeningDrawer != null ? null : (ModelDrawable<Profile>)this;

                    listening = Main.ListeningDrawer != null;
                }
                GUILayout.Space(10);
                if(Drawer.Button(!configMode ? string.Format(Main.Lang.Get("ENABLE", "Enable {0}"), Main.Lang.Get("CONFIGURATION_MODE", "Configuration Mode")) : string.Format(Main.Lang.Get("DISABLE", "Disable {0}"), Main.Lang.Get("CONFIGURATION_MODE", "Configuration Mode")))) {
                    configMode = !configMode;
                }
                if(!model.Keys.Any(k => k.Code == KeyCode.Mouse0)) {
                    GUILayout.Space(10);
                    if(Drawer.Button(Main.Lang.Get("REGISTER_MOUSE0_KEY", "Register Left Click Key"))) {
                        model.Keys.Add(new KeyConfig() { Code = KeyCode.Mouse0 });
                        manager.UpdateKeys();
                    }
                }
                GUILayout.Space(10);
                if(Drawer.Button(Main.Lang.Get("CREATE_DUMMY_KEY", "Create Dummy Key"))) {
                    var dummy = new KeyConfig() { DummyName = string.Format(Main.Lang.Get("DUMMY_NAME", "Dummy {0}"), dummyNumber++) };
                    model.Keys.Add(dummy);
                    manager.UpdateKeys();
                }
                if(selectedKeys.Count == 2) {
                    GUILayout.Space(10);
                    if(Drawer.Button(Main.Lang.Get("SWAP_KEYS", "Swap Keys Order"))) {
                        var list = selectedKeys.ToList();
                        int a = model.Keys.IndexOf(list[0]);
                        int b = model.Keys.IndexOf(list[1]);
                        (model.Keys[b], model.Keys[a]) = (model.Keys[a], model.Keys[b]);
                        manager.UpdateKeys();
                        selectedKeys.Clear();
                        criterion = null;
                    }
                }
                if(selectedKeys.Count > 0) {
                    GUILayout.Space(10);
                    if(Drawer.Button(Main.Lang.Get("MAKE_BAR", "Make Bar Key"))) {
                        KeyViewerUtils.MakeBar(manager.profile, manager.keys.FindAll(k => selectedKeys.Contains(k.Config)).Select(k => k.Config).ToList());
                        manager.UpdateLayout();
                        selectedKeys.Clear();
                        criterion = null;
                    }
                }
                if(selectedKeys.Count > 1) {
                    GUILayout.Space(10);
                    if(Drawer.Button(Main.Lang.Get("EDIT_MULTIPLE_KEY", "Edit Multiple Keys"))) {
                        Main.GUI.Push(new MultipleKeyConfigDrawer(manager, selectedKeys.Select(KeyViewerUtils.KeyName).ToList(), criterion?.Copy()));
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
