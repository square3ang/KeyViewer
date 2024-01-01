using KeyViewer.Core.Interfaces;
using KeyViewer.Unity;
using System;
using UnityEngine;

namespace KeyViewer.Core
{
    public abstract class ModelDrawable<T> : IDrawable where T : IModel
    {
        public T model;
        public ModelDrawable(T model)
        {
            this.model = model;
        }
        public abstract void Draw(IDrawer drawer);
        public virtual void OnKeyDown(KeyCode code) { }
        protected static string L(string translationKey, params object[] formatArgs)
        {
            if (formatArgs.Length == 0)
                return Main.Lang[translationKey];
            else return string.Format(Main.Lang[translationKey], formatArgs);
        }
        protected static void BeginIndent()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
        }
        protected static void EndIndent()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        protected static void ExpandableGUI(Action en, string title, ref bool enabled, ref bool expanded)
        {
            ExpandableGUI(en, null, title, ref enabled, ref expanded);
        }
        protected static void ExpandableGUI(Action en, Action dis, string title, ref bool enabled, ref bool expanded)
        {
            ExpandableGUI(en, dis, null, null, null, null, title, null, ref enabled, ref expanded);
        }
        protected static void ExpandableGUI(Action enGui, Action disGui, Action onEnable, Action onDisable, Action onHide, Action onShow, string title, string desc, ref bool enabled, ref bool expanded)
        {
            GUILayout.BeginHorizontal();
            bool newIsExpanded = GUILayout.Toggle(
            expanded,
            disGui == null ? 
                (enabled ? (expanded ? "◢" : "▶") : "") : 
                (expanded ? "◢" : "▶"),
            new GUIStyle()
                {
                    fixedWidth = 10,
                    normal = new GUIStyleState() { textColor = Color.white },
                    fontSize = 15,
                    margin = new RectOffset(4, 2, 6, 6),
                });
            bool newIsEnabled = GUILayout.Toggle(
                enabled,
                title,
                new GUIStyle(GUI.skin.toggle)
                {
                    fontStyle = FontStyle.Normal,
                    font = null,
                    margin = new RectOffset(0, 4, 4, 4),
                });
            if (!string.IsNullOrEmpty(desc)) 
            {
                GUILayout.Label("-");
                GUILayout.Label(
                    desc,
                    new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Handle enable/disable change
            if (newIsEnabled != enabled)
            {
                enabled = newIsEnabled;
                if (newIsEnabled)
                {
                    onEnable?.Invoke();
                    newIsExpanded = true;
                }
                else onDisable?.Invoke();
            }

            // Handle expand/collapse change
            if (newIsExpanded != expanded)
            {
                expanded = newIsExpanded;
                if (!newIsExpanded) onHide?.Invoke();
                else onShow?.Invoke();
            }

            // Draw custom options
            if (expanded)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(24f);
                GUILayout.BeginVertical();
                if (enabled) enGui?.Invoke();
                else disGui?.Invoke();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(12f);
            }
        }
    }
}
