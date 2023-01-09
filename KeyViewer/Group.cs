﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static KeyViewer.Main;

namespace KeyViewer
{
    public class Group
    {
        public Group() { }
        internal KeyManager keyManager;
        public Group(KeyManager keyManager, string name)
        {
            this.keyManager = keyManager;
            Name = name;
            groupConfig = new Key.Config(keyManager);
        }
        public List<KeyCode> codes = new List<KeyCode>();
        public Key.Config groupConfig;
        public string Name = "Group";
        public bool Editing = false;
        private bool isResolved = false;
        public void AddConfig(Key.Config config)
        {
            configs.Add(config);
            codes.Add(config.Code);
        }
        public bool RemoveConfig(Key.Config config)
        {
            bool toRet = configs.Remove(config);
            toRet &= codes.Remove(config.Code);
            return toRet;
        }
        public void Resolve()
        {
            configs.Clear();
            for (int i = 0; i < codes.Count; i++)
                configs.Add(keyManager.keys[codes[i]].config);
            groupConfig.Initialized = true;
            isResolved = true;
        }
        public bool IsAdded(Key.Config config) => codes.Contains(config.Code);
        private List<Key.Config> configs = new List<Key.Config>();
        public void RenderGUI()
        {
            if (!isResolved)
                Resolve();
            MoreGUILayout.BeginIndent();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Lang.GetString("DUPLICATE")))
                Main.Settings.CurrentProfile.KeyGroups.Add(Copy());
            if (GUILayout.Button(Lang.GetString("DELETE")))
                Main.Settings.CurrentProfile.KeyGroups.Remove(this);
            GUILayout.Label("Codes: ");
            for (int i = 0; i < configs.Count; i++)
                GUILayout.Label($"{configs[i].Code} ");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            Name = MoreGUILayout.NamedTextField(Lang.GetString("KEY_GROUP_NAME"), Name, 400f);
            Key.DrawGlobalConfig(groupConfig, c =>
            {
                configs.ForEach(conf => conf.ApplyConfig(c));
                keyManager.UpdateLayout();
            });
            MoreGUILayout.EndIndent();
        }
        public Group Copy()
        {
            Group g = new Group(keyManager, Name + " Copy");
            g.configs.AddRange(configs);
            return g;
        }
    }
}
