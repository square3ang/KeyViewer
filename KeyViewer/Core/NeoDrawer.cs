using KeyViewer;
using KeyViewer.Core;
using KeyViewer.Models;
using NCalc;
using Newtonsoft.Json.Linq;
using RapidGUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static HarmonyLib.AccessTools;

namespace Overlayer.Core {
    public class NeoDrawer {
        public static NeoDrawer StaticInstance = new NeoDrawer();
        public class NeoField {
            public enum StateType {
                OK = 0,
                ERROR = 1,
                WARNING = 2,
                COMPUTE = 3,
            }

            public bool StrInitialized = false;
            public string Str;
            public StateType State;
            public object ComputedValue;

        }

        private string LastFocused;

        private uint id = 0;
        private Dictionary<string, NeoField> fields = new();

        public bool StrInitialize(ref NeoField field, string str) {
            if(!field.StrInitialized) {
                field.Str = str;
                field.StrInitialized = true;
                return true;
            }
            return false;
        }

        public uint FieldGetId() {
            return id;
        }
        public void FieldSetId(uint value) {
            id = value;
        }
        public void FieldIncId() {
            id++;
        }

        public void FieldResetId() {
            id = 0;
        }
        public void FieldResetDictById() {
            var keysToRemove = fields.Keys
                .Where(k => uint.TryParse(k, out _))
                .ToList();

            foreach(var key in keysToRemove) {
                fields.Remove(key);
            }
        }
        public void FieldClear() {
            id = 0;
            fields.Clear();
        }

        public NeoField FieldGet(string uniqueID = null) {
            string key = uniqueID ?? id++.ToString();
            if(!fields.TryGetValue(key, out NeoField field)) {
                field = new NeoField();
                fields[key] = field;
            }
            return field;
        }

        public string FieldGetName(string uniqueID = null) {
            return $"Field_{uniqueID ?? (id - 1).ToString()}";
        }

        public void FieldsRemove(params string[] keys) {
            foreach(var key in keys) {
                fields.Remove(key);
            }
        }

        public void UpdateFocused() {
            LastFocused = GUI.GetNameOfFocusedControl();
        }

        public object Calc(string exprStr) {
            var expr = new Expression(exprStr);

            expr.EvaluateParameter += (name, args) => {
                switch(name.ToUpperInvariant()) {
                    case "PI":
                        args.Result = Math.PI;
                        break;
                    case "E":
                        args.Result = Math.E;
                        break;
                }
            };

            try {
                return expr.Evaluate();
            } catch {
                return null;
            }
        }

        private bool ApplyFieldValueOnEvent(ref NeoField field, string fieldName, ref object value, Type type) {
            string focused = GUI.GetNameOfFocusedControl();

            bool shouldApply =
                ((focused == fieldName && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)
                || (LastFocused != fieldName))
                && (field.State == NeoField.StateType.COMPUTE || field.State == NeoField.StateType.WARNING);

            if(shouldApply) {
                try {
                    switch(Type.GetTypeCode(type)) {
                        case TypeCode.Boolean:
                            value = Convert.ToBoolean(field.ComputedValue);
                            break;
                        case TypeCode.Char:
                            value = Convert.ToChar(field.ComputedValue);
                            break;
                        case TypeCode.SByte:
                            value = Convert.ToSByte(field.ComputedValue);
                            break;
                        case TypeCode.Byte:
                            value = Convert.ToByte(field.ComputedValue);
                            break;
                        case TypeCode.Int16:
                            value = Convert.ToInt16(field.ComputedValue);
                            break;
                        case TypeCode.UInt16:
                            value = Convert.ToUInt16(field.ComputedValue);
                            break;
                        case TypeCode.Int32:
                            value = Convert.ToInt32(field.ComputedValue);
                            break;
                        case TypeCode.UInt32:
                            value = Convert.ToUInt32(field.ComputedValue);
                            break;
                        case TypeCode.Int64:
                            value = Convert.ToInt64(field.ComputedValue);
                            break;
                        case TypeCode.UInt64:
                            value = Convert.ToUInt64(field.ComputedValue);
                            break;
                        case TypeCode.Single:
                            value = Convert.ToSingle(field.ComputedValue);
                            break;
                        case TypeCode.Double:
                            value = Convert.ToDouble(field.ComputedValue);
                            break;
                        case TypeCode.Decimal:
                            value = Convert.ToDecimal(field.ComputedValue);
                            break;
                        case TypeCode.String:
                            if(float.TryParse(Convert.ToString(field.ComputedValue), out float parsed))
                                value = parsed;
                            else
                                field.State = NeoField.StateType.ERROR;
                            break;
                        default:
                            field.State = NeoField.StateType.ERROR;
                            return false;
                    }

                    field.Str = value.ToString();
                    field.State = NeoField.StateType.OK;
                    return true;
                } catch {
                    field.State = NeoField.StateType.ERROR;
                    return false;
                }
            }

            return false;
        }

        public void ColorbyState(NeoField.StateType state) {
            switch(state) {
                case NeoField.StateType.ERROR:
                    GUI.color = new Color(1f, 0.5f, 0.5f);
                    break;
                case NeoField.StateType.WARNING:
                    GUI.color = new Color(1f, 1f, 0.5f);
                    break;
                case NeoField.StateType.COMPUTE:
                    GUI.color = new Color(0.5f, 1f, 0.5f);
                    break;
                default:
                    GUI.color = Color.white;
                    break;
            }
        }

        public string StatebyState(NeoField.StateType state) {
            switch(state) {
                case NeoField.StateType.ERROR:
                    return "<color=#FF8888>!!</color>";
                case NeoField.StateType.WARNING:
                    return "<color=#FFFF88>!</color>";
                case NeoField.StateType.COMPUTE:
                    return "<color=#88FF88>✓</color>";
                default:
                    return "";
            }
        }

        public bool DrawVector3(string label, ref Vector3 vec3, float lValue, float rValue, string uniqueID = null) {
            bool changed = false;
            GUILayout.Label($"<b>{label}</b>");
            if(uniqueID == null) {
                changed |= DrawSingleWithSlider("X", ref vec3.x, lValue, rValue, 300f);
                changed |= DrawSingleWithSlider("Y", ref vec3.y, lValue, rValue, 300f);
                changed |= DrawSingleWithSlider("Z", ref vec3.z, lValue, rValue, 300f);
            } else {
                changed |= DrawSingleWithSlider("X", ref vec3.x, lValue, rValue, 300f, uniqueID + "_0");
                changed |= DrawSingleWithSlider("Y", ref vec3.y, lValue, rValue, 300f, uniqueID + "_1");
                changed |= DrawSingleWithSlider("Z", ref vec3.z, lValue, rValue, 300f, uniqueID + "_2");
            }
            return changed;
        }

        public bool DrawRotate3(string label, ref Vector3 vec3, float lValue, float rValue, string uniqueID = null) {
            bool changed = false;
            GUILayout.Label($"<b>{label}</b>");
            Color old = GUI.color;
            if(uniqueID == null) {
                GUI.color = new Color(1.0f, 0.68f, 0.68f);
                changed |= DrawSingleWithSlider(Drawer.Icon_XRotate, "X", ref vec3.x, lValue, rValue, 300f);
                GUI.color = new Color(0.68f, 1.0f, 0.68f);
                changed |= DrawSingleWithSlider(Drawer.Icon_YRotate, "Y", ref vec3.y, lValue, rValue, 300f);
                GUI.color = new Color(0.68f, 0.68f, 1.0f);
                changed |= DrawSingleWithSlider(Drawer.Icon_ZRotate, "Z", ref vec3.z, lValue, rValue, 300f);
            } else {
                GUI.color = new Color(1.0f, 0.68f, 0.68f);
                changed |= DrawSingleWithSlider(Drawer.Icon_XRotate, "X", ref vec3.x, lValue, rValue, 300f, uniqueID + "_0");
                GUI.color = new Color(0.68f, 1.0f, 0.68f);
                changed |= DrawSingleWithSlider(Drawer.Icon_YRotate, "Y", ref vec3.y, lValue, rValue, 300f, uniqueID + "_1");
                GUI.color = new Color(0.68f, 0.68f, 1.0f);
                changed |= DrawSingleWithSlider(Drawer.Icon_ZRotate, "Z", ref vec3.z, lValue, rValue, 300f, uniqueID + "_2");
            }
            GUI.color = old;
            return changed;
        }

        public bool DrawVector2(string label, ref Vector2 vec2, float lValue, float rValue, string uniqueID = null) {
            bool changed = false;
            GUILayout.Label($"<b>{label}</b>");
            if(uniqueID == null) {
                changed |= DrawSingleWithSlider("X", ref vec2.x, lValue, rValue, 300f);
                changed |= DrawSingleWithSlider("Y", ref vec2.y, lValue, rValue, 300f);
            } else {
                changed |= DrawSingleWithSlider("X", ref vec2.x, lValue, rValue, 300f, uniqueID + "_0");
                changed |= DrawSingleWithSlider("Y", ref vec2.y, lValue, rValue, 300f, uniqueID + "_1");
            }
            return changed;
        }

        public bool DrawSize2(string label, ref Vector2 vec2, float lValue, float rValue, string uniqueID = null) {
            bool changed = false;
            GUILayout.Label($"<b>{label}</b>");
            Color old = GUI.color;
            if(uniqueID == null) {
                GUI.color = new Color(1.0f, 0.68f, 0.68f);
                changed |= DrawSingleWithSlider(Drawer.Icon_LeftRight, "X", ref vec2.x, lValue, rValue, 300f);
                GUI.color = new Color(0.68f, 1.0f, 0.68f);
                changed |= DrawSingleWithSlider(Drawer.Icon_UpDown, "Y", ref vec2.y, lValue, rValue, 300f);
            } else {
                GUI.color = new Color(1.0f, 0.68f, 0.68f);
                changed |= DrawSingleWithSlider(Drawer.Icon_LeftRight, "X", ref vec2.x, lValue, rValue, 300f, uniqueID + "_0");
                GUI.color = new Color(0.68f, 1.0f, 0.68f);
                changed |= DrawSingleWithSlider(Drawer.Icon_UpDown, "Y", ref vec2.y, lValue, rValue, 300f, uniqueID + "_1");
            }
            GUI.color = old;
            return changed;
        }

        public bool DrawColor(string label, ref Color color, float cWidth = 460f, string uniqueID = null) {
            bool changed = false;

            NeoField field = FieldGet(uniqueID);
            StrInitialize(ref field, ColorUtility.ToHtmlStringRGBA(color));

            GUILayout.BeginHorizontal();
            if(!string.IsNullOrEmpty(label)) {
                GUILayout.Label(label);
                GUILayout.Space(4f);
            }

            Color old = GUI.color;
            if(field.State == NeoField.StateType.ERROR) {
                GUI.color = new Color(1f, 0.5f, 0.5f);
            } else {
                GUI.color = Color.white;
            }

            GUI.SetNextControlName(FieldGetName(uniqueID));
            string newHex = GUILayout.TextField(field.Str, 8, Drawer.myTextFieldNoPad, GUILayout.Width(80f));

            if(newHex != field.Str) {
                field.Str = newHex;
                changed = true;

                if(ColorUtility.TryParseHtmlString("#" + field.Str, out Color parsed)) {
                    color = parsed;
                    field.State = NeoField.StateType.OK;
                } else {
                    field.State = NeoField.StateType.ERROR;
                }
            }

            GUI.color = old;
            GUILayout.Space(2f);

            GUILayout.Label(StatebyState(field.State), GUILayout.Width(10));

            Color newColor = RGUI.Field(color, "", GUILayout.Width(cWidth));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if(newColor != color) {
                color = newColor;
                changed = true;
                field.Str = ColorUtility.ToHtmlStringRGBA(color);
                field.State = NeoField.StateType.OK;
            }

            return changed;
        }

        public bool DrawGColor(ref GColor color, bool canEnableGradient, float cWidth = 220f, string uniqueID = null) {
            bool prevGe = color.gradientEnabled;
            bool ge = prevGe;

            if(canEnableGradient && Drawer.DrawBool(Drawer.Icon_Gradation, Main.Lang.Get("MISC_ENABLE_GRADIENT", "Enable Gradient"), ref ge)) {
                color = color with { gradientEnabled = ge };
            }

            color = color with { gradientEnabled = color.gradientEnabled && canEnableGradient };

            bool changed = ge != prevGe;

            if(color.gradientEnabled && canEnableGradient) {

                NeoField fieldTL = FieldGet(uniqueID);
                NeoField fieldTR;
                NeoField fieldBL;
                NeoField fieldBR;

                if(string.IsNullOrEmpty(uniqueID)) {
                    fieldTR = FieldGet();
                    fieldBL = FieldGet();
                    fieldBR = FieldGet();
                } else {
                    fieldTR = FieldGet(uniqueID + "_1");
                    fieldBL = FieldGet(uniqueID + "_2");
                    fieldBR = FieldGet(uniqueID + "_3");
                }

                StrInitialize(ref fieldTL, color.topLeftHex);
                StrInitialize(ref fieldTR, color.topRightHex);
                StrInitialize(ref fieldBL, color.bottomLeftHex);
                StrInitialize(ref fieldBR, color.bottomRightHex);

                if(changed && ge) {
                    fieldTL.Str = color.topLeftHex;
                    fieldTR.Str = color.topRightHex;
                    fieldBL.Str = color.bottomLeftHex;
                    fieldBR.Str = color.bottomRightHex;

                    fieldTL.State = NeoField.StateType.OK;
                    fieldTR.State = NeoField.StateType.OK;
                    fieldBL.State = NeoField.StateType.OK;
                    fieldBR.State = NeoField.StateType.OK;
                }

                /* ! TOP ! */

                GUILayout.BeginHorizontal();

                // TL
                Color newColorTL = RGUI.Field(color.topLeft, "", GUILayout.Width(cWidth));
                GUILayout.Space(2f);

                Color old = GUI.color;
                if(fieldTL.State == NeoField.StateType.ERROR) {
                    GUI.color = new Color(1f, 0.5f, 0.5f);
                }

                GUI.SetNextControlName(FieldGetName(uniqueID));
                string newHexTL = GUILayout.TextField(fieldTL.Str, 8, Drawer.myTextFieldNoPad, GUILayout.Width(80f));
                GUI.color = old;

                if(newHexTL != fieldTL.Str) {
                    fieldTL.Str = newHexTL;
                    changed = true;

                    if(ColorUtility.TryParseHtmlString("#" + fieldTL.Str, out Color parsed)) {
                        color.topLeft = parsed;
                        fieldTL.State = NeoField.StateType.OK;
                    } else {
                        fieldTL.State = NeoField.StateType.ERROR;
                    }
                }

                if(newColorTL != color.topLeft) {
                    color.topLeft = newColorTL;
                    changed = true;

                    fieldTL.Str = color.topLeftHex;
                    fieldTL.State = NeoField.StateType.OK;
                }

                GUILayout.Space(4f);
                GUILayout.Label("↖", GUILayout.Width(16));

                // TR
                if(fieldTR.State == NeoField.StateType.ERROR) {
                    GUI.color = new Color(1f, 0.5f, 0.5f);
                }

                GUILayout.Label("↗", GUILayout.Width(16));
                GUI.SetNextControlName(FieldGetName(uniqueID + "_1"));
                string newHexTR = GUILayout.TextField(fieldTR.Str, 8, Drawer.myTextFieldNoPad, GUILayout.Width(80f));
                GUI.color = old;

                if(newHexTR != fieldTR.Str) {
                    fieldTR.Str = newHexTR;
                    changed = true;

                    if(ColorUtility.TryParseHtmlString("#" + fieldTR.Str, out Color parsed)) {
                        color.topRight = parsed;
                        fieldTR.State = NeoField.StateType.OK;
                    } else {
                        fieldTR.State = NeoField.StateType.ERROR;
                    }
                }

                Color newColorTR = RGUI.Field(color.topRight, "", GUILayout.Width(cWidth));
                if(newColorTR != color.topRight) {
                    color.topRight = newColorTR;
                    changed = true;

                    fieldTR.Str = color.topLeftHex;
                    fieldTR.State = NeoField.StateType.OK;
                }

                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();

                /* ! BOTTOM ! */

                GUILayout.BeginHorizontal();

                // BL
                Color newColorBL = RGUI.Field(color.bottomLeft, "", GUILayout.Width(cWidth));
                GUILayout.Space(2f);

                if(fieldBL.State == NeoField.StateType.ERROR) {
                    GUI.color = new Color(1f, 0.5f, 0.5f);
                }

                GUI.SetNextControlName(FieldGetName(uniqueID + "_2"));
                string newHexBL = GUILayout.TextField(fieldBL.Str, 8, Drawer.myTextFieldNoPad, GUILayout.Width(80f));
                GUI.color = old;

                if(newHexBL != fieldBL.Str) {
                    fieldBL.Str = newHexBL;
                    changed = true;

                    if(ColorUtility.TryParseHtmlString("#" + fieldBL.Str, out Color parsed)) {
                        color.bottomLeft = parsed;
                        fieldBL.State = NeoField.StateType.OK;
                    } else {
                        fieldBL.State = NeoField.StateType.ERROR;
                    }
                }

                if(newColorBL != color.bottomLeft) {
                    color.bottomLeft = newColorBL;
                    changed = true;

                    fieldBL.Str = color.topLeftHex;
                    fieldBL.State = NeoField.StateType.OK;
                }

                GUILayout.Space(4f);
                GUILayout.Label("↙", GUILayout.Width(16));

                // BR
                if(fieldBR.State == NeoField.StateType.ERROR) {
                    GUI.color = new Color(1f, 0.5f, 0.5f);
                }

                GUILayout.Label("↘", GUILayout.Width(16));
                GUI.SetNextControlName(FieldGetName(uniqueID + "_3"));
                string newHexBR = GUILayout.TextField(fieldBR.Str, 8, Drawer.myTextFieldNoPad, GUILayout.Width(80f));
                GUI.color = old;

                if(newHexBR != fieldBR.Str) {
                    fieldBR.Str = newHexBR;
                    changed = true;

                    if(ColorUtility.TryParseHtmlString("#" + fieldBR.Str, out Color parsed)) {
                        color.bottomRight = parsed;
                        fieldBR.State = NeoField.StateType.OK;
                    } else {
                        fieldBR.State = NeoField.StateType.ERROR;
                    }
                }

                Color newColorBR = RGUI.Field(color.bottomRight, "", GUILayout.Width(cWidth));
                if(newColorBR != color.bottomRight) {
                    color.bottomRight = newColorBR;
                    changed = true;

                    fieldBR.Str = color.topLeftHex;
                    fieldBR.State = NeoField.StateType.OK;
                }

                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            } else {
                Color all = color.topLeft;
                if(changed = DrawColor("", ref all, cWidth, uniqueID)) {
                    color = all;
                }
            }
            return changed;
        }

        public bool DrawSingle(string label, ref float value, string uniqueID = null) {
            NeoField field = FieldGet(uniqueID);
            StrInitialize(ref field, value.ToString());

            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            GUILayout.Space(4f);

            bool changed = false;

            Color old = GUI.color;
            ColorbyState(field.State);

            string fieldName = FieldGetName(uniqueID);
            GUI.SetNextControlName(fieldName);
            string newField = GUILayout.TextField(field.Str, Drawer.myTextField);
            GUI.color = old;

            if(newField != field.Str) {
                field.Str = newField;
                if(string.IsNullOrEmpty(field.Str)) {
                    field.State = NeoField.StateType.ERROR;
                } else {
                    if(float.TryParse(newField, out float parsed)) {
                        value = parsed;
                        field.ComputedValue = parsed;
                        field.State = NeoField.StateType.OK;
                        changed = true;
                    } else {
                        var result = Calc(field.Str);
                        if(result == null) {
                            field.State = NeoField.StateType.ERROR;
                        } else {
                            float computed = Convert.ToSingle(result);
                            field.ComputedValue = computed;
                            field.State = (float.IsNaN(computed) || float.IsInfinity(computed))
                                ? NeoField.StateType.WARNING
                                : NeoField.StateType.COMPUTE;
                        }
                    }
                }
            }

            object objValue = value;
            if(ApplyFieldValueOnEvent(ref field, fieldName, ref objValue, typeof(float))) {
                value = (float)objValue;
                changed = true;
            }

            GUILayout.Space(2f);
            GUILayout.Label(StatebyState(field.State), GUILayout.Width(12));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return changed;
        }

        public bool DrawSingleWithSlider(string label, ref float value, float lValue, float rValue, float width, string uniqueID = null) {
            NeoField field = FieldGet(uniqueID);
            StrInitialize(ref field, value.ToString());

            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            GUILayout.Space(4f);

            bool changed = false;

            float sliderValue = GUILayout.HorizontalSlider(value, lValue, rValue, Drawer.mySlider, Drawer.myThumb, GUILayout.Width(width));
            if(sliderValue != value) {
                value = sliderValue;
                field.Str = value.ToString();
                field.State = NeoField.StateType.OK;
                changed = true;
            }

            GUILayout.Space(8f);

            Color old = GUI.color;
            ColorbyState(field.State);

            string fieldName = FieldGetName(uniqueID);
            GUI.SetNextControlName(fieldName);
            string newField = GUILayout.TextField(field.Str, Drawer.myTextField);
            GUI.color = old;

            if(newField != field.Str) {
                field.Str = newField;
                if(string.IsNullOrEmpty(field.Str)) {
                    field.State = NeoField.StateType.ERROR;
                } else {
                    if(float.TryParse(newField, out float parsed)) {
                        value = parsed;
                        field.ComputedValue = parsed;
                        field.State = NeoField.StateType.OK;
                        changed = true;
                    } else {
                        var result = Calc(field.Str);
                        if(result == null) {
                            field.State = NeoField.StateType.ERROR;
                        } else {
                            float computed = Convert.ToSingle(result);
                            field.ComputedValue = computed;
                            field.State = (float.IsNaN(computed) || float.IsInfinity(computed))
                                ? NeoField.StateType.WARNING
                                : NeoField.StateType.COMPUTE;
                        }
                    }
                }
            }

            object objValue = value;
            if(ApplyFieldValueOnEvent(ref field, fieldName, ref objValue, typeof(float))) {
                value = (float)objValue;
                changed = true;
            }

            GUILayout.Space(2f);
            GUILayout.Label(StatebyState(field.State), GUILayout.Width(12));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return changed;
        }

        public bool DrawSingleWithSlider(Texture2D icon, string label, ref float value, float lValue, float rValue, float width, string uniqueID = null) {
            NeoField field = FieldGet(uniqueID);
            StrInitialize(ref field, value.ToString());

            GUILayout.BeginHorizontal();
            GUILayout.Label(icon);
            GUILayout.Label(label);
            GUILayout.Space(4f);

            bool changed = false;

            float sliderValue = GUILayout.HorizontalSlider(value, lValue, rValue, Drawer.mySlider, Drawer.myThumb, GUILayout.Width(width));
            if(sliderValue != value) {
                value = sliderValue;
                field.Str = value.ToString();
                field.State = NeoField.StateType.OK;
                changed = true;
            }

            GUILayout.Space(8f);

            Color old = GUI.color;
            ColorbyState(field.State);

            string fieldName = FieldGetName(uniqueID);
            GUI.SetNextControlName(fieldName);
            string newField = GUILayout.TextField(field.Str, Drawer.myTextField);
            GUI.color = old;

            if(newField != field.Str) {
                field.Str = newField;
                if(string.IsNullOrEmpty(field.Str)) {
                    field.State = NeoField.StateType.ERROR;
                } else {
                    if(float.TryParse(newField, out float parsed)) {
                        value = parsed;
                        field.ComputedValue = parsed;
                        field.State = NeoField.StateType.OK;
                        changed = true;
                    } else {
                        var result = Calc(field.Str);
                        if(result == null) {
                            field.State = NeoField.StateType.ERROR;
                        } else {
                            float computed = Convert.ToSingle(result);
                            field.ComputedValue = computed;
                            field.State = (float.IsNaN(computed) || float.IsInfinity(computed))
                                ? NeoField.StateType.WARNING
                                : NeoField.StateType.COMPUTE;
                        }
                    }
                }
            }

            object objValue = value;
            if(ApplyFieldValueOnEvent(ref field, fieldName, ref objValue, typeof(float))) {
                value = (float)objValue;
                changed = true;
            }

            GUILayout.Space(2f);
            GUILayout.Label(StatebyState(field.State), GUILayout.Width(12));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return changed;
        }

        public bool DrawDouble(string label, ref double value, string uniqueID = null) {
            NeoField field = FieldGet(uniqueID);
            StrInitialize(ref field, value.ToString());

            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            GUILayout.Space(4f);

            bool changed = false;

            Color old = GUI.color;
            ColorbyState(field.State);

            string fieldName = FieldGetName(uniqueID);
            GUI.SetNextControlName(fieldName);
            string newField = GUILayout.TextField(field.Str, Drawer.myTextField);
            GUI.color = old;

            if(newField != field.Str) {
                field.Str = newField;
                if(string.IsNullOrEmpty(field.Str)) {
                    field.State = NeoField.StateType.ERROR;
                } else {
                    if(double.TryParse(newField, out double parsed)) {
                        value = parsed;
                        field.ComputedValue = parsed;
                        field.State = NeoField.StateType.OK;
                        changed = true;
                    } else {
                        var result = Calc(field.Str);
                        if(result == null) {
                            field.State = NeoField.StateType.ERROR;
                        } else {
                            double computed = Convert.ToSingle(result);
                            field.ComputedValue = computed;
                            field.State = (double.IsNaN(computed) || double.IsInfinity(computed))
                                ? NeoField.StateType.WARNING
                                : NeoField.StateType.COMPUTE;
                        }
                    }
                }
            }

            object objValue = value;
            if(ApplyFieldValueOnEvent(ref field, fieldName, ref objValue, typeof(double))) {
                value = (double)objValue;
                changed = true;
            }

            GUILayout.Space(2f);
            GUILayout.Label(StatebyState(field.State), GUILayout.Width(12));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return changed;
        }

        public bool DrawInt32(string label, ref int value, string uniqueID = null) {
            NeoField field = FieldGet(uniqueID);
            StrInitialize(ref field, value.ToString());

            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            GUILayout.Space(4f);

            bool changed = false;

            Color old = GUI.color;
            ColorbyState(field.State);

            string fieldName = FieldGetName(uniqueID);
            GUI.SetNextControlName(fieldName);
            string newField = GUILayout.TextField(field.Str, Drawer.myTextField);
            GUI.color = old;

            if(newField != field.Str) {
                field.Str = newField;
                if(string.IsNullOrEmpty(field.Str)) {
                    field.State = NeoField.StateType.ERROR;
                } else {
                    if(int.TryParse(newField, out int parsed)) {
                        value = parsed;
                        field.ComputedValue = parsed;
                        field.State = NeoField.StateType.OK;
                        changed = true;
                    } else {
                        var result = Calc(field.Str);
                        if(result == null) {
                            field.State = NeoField.StateType.ERROR;
                        } else {
                            double computed = Convert.ToDouble(result);
                            if(double.IsNaN(computed) || double.IsInfinity(computed)) {
                                field.State = NeoField.StateType.ERROR;
                            } else if(computed > int.MaxValue) {
                                field.State = NeoField.StateType.WARNING;
                                field.ComputedValue = int.MaxValue;
                            } else if(computed < int.MinValue) {
                                field.State = NeoField.StateType.WARNING;
                                field.ComputedValue = int.MinValue;
                            } else {
                                int computedInt = (int)Math.Round(computed);
                                field.ComputedValue = computedInt;
                                field.State = NeoField.StateType.COMPUTE;
                            }
                        }
                    }
                }
            }

            object objValue = value;
            if(ApplyFieldValueOnEvent(ref field, fieldName, ref objValue, typeof(int))) {
                value = (int)objValue;
                changed = true;
            }

            GUILayout.Space(2f);
            GUILayout.Label(StatebyState(field.State), GUILayout.Width(12));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return changed;
        }

        public bool DrawInt32WithSlider(Texture2D icon, string label, ref int value, int lValue, int rValue, float width, string uniqueID = null) {
            NeoField field = FieldGet(uniqueID);
            StrInitialize(ref field, value.ToString());

            GUILayout.BeginHorizontal();
            GUILayout.Label(icon);
            GUILayout.Label(label);
            GUILayout.Space(4f);

            bool changed = false;

            float slider = GUILayout.HorizontalSlider(value, lValue, rValue, Drawer.mySlider, Drawer.myThumb, GUILayout.Width(width));

            int sliderInt = Mathf.RoundToInt(slider);
            if(sliderInt != value) {
                value = sliderInt;
                field.Str = value.ToString();
                field.State = NeoField.StateType.OK;
                changed = true;
            }

            GUILayout.Space(8f);

            Color old = GUI.color;
            ColorbyState(field.State);

            string fieldName = FieldGetName(uniqueID);
            GUI.SetNextControlName(fieldName);
            string newField = GUILayout.TextField(field.Str, Drawer.myTextField);
            GUI.color = old;

            if(newField != field.Str) {
                field.Str = newField;

                if(string.IsNullOrEmpty(field.Str)) {
                    field.State = NeoField.StateType.ERROR;
                } else {
                    if(int.TryParse(newField, out int parsed)) {
                        value = parsed;
                        field.ComputedValue = parsed;
                        field.State = NeoField.StateType.OK;
                        changed = true;
                    } else {
                        var result = Calc(field.Str);
                        if(result == null) {
                            field.State = NeoField.StateType.ERROR;
                        } else {
                            double computed = Convert.ToDouble(result);

                            if(double.IsNaN(computed) || double.IsInfinity(computed)) {
                                field.State = NeoField.StateType.ERROR;
                            } else if(computed > rValue) {
                                value = rValue;
                                field.State = NeoField.StateType.WARNING;
                                changed = true;
                            } else if(computed < lValue) {
                                value = lValue;
                                field.State = NeoField.StateType.WARNING;
                                changed = true;
                            } else {
                                int computedInt = (int)Math.Round(computed);
                                value = computedInt;
                                field.ComputedValue = computedInt;
                                field.State = NeoField.StateType.COMPUTE;
                                changed = true;
                            }
                        }
                    }
                }
            }

            object objValue = value;
            if(ApplyFieldValueOnEvent(ref field, fieldName, ref objValue, typeof(int))) {
                value = (int)objValue;
                changed = true;
            }

            GUILayout.Space(2f);
            GUILayout.Label(StatebyState(field.State), GUILayout.Width(12));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return changed;
        }


        public bool DrawBlurConfig(BlurConfig blurConfig, string uniqueID = null) {
            bool changed = false;
            GUILayout.Label($"<b>{Main.Lang.Get("BACKGROUND_BLUR", "Background Blur")}</b>");
            Color old = GUI.color;
            if(uniqueID == null) {
                GUI.color = new Color(1.0f, 0.55f, 0.25f);
                changed |= DrawSingleWithSlider(Drawer.Icon_Blur, "S", ref blurConfig.Spacing, 0, 40, 300f);
                GUI.color = new Color(0.35f, 0.9f, 1.0f);
                changed |= DrawSingleWithSlider(Drawer.Icon_Sun, "V", ref blurConfig.Vibrancy, 0, 2, 300f);
            } else {
                GUI.color = new Color(1.0f, 0.55f, 0.25f);
                changed |= DrawSingleWithSlider(Drawer.Icon_Blur, "S", ref blurConfig.Spacing, 0, 40, 300f, uniqueID + "_0");
                GUI.color = new Color(0.35f, 0.9f, 1.0f);
                changed |= DrawSingleWithSlider(Drawer.Icon_Sun, "V", ref blurConfig.Vibrancy, 0, 2, 300f, uniqueID + "_1");
            }
            GUI.color = old;
            return changed;
        }

        public bool DrawVectorConfig(VectorConfig vConfig) {
            bool changed = false;
            Color old = GUI.color;

            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Scale);
            GUILayout.Label($"<b>{Main.Lang.Get("SCALE", "Scale")}</b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Down);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                vConfig.Scale.Pressed = vConfig.Scale.Released;
                vConfig.Scale.PressedEase = vConfig.Scale.ReleasedEase.Copy();
                FieldGet(id.ToString())?.Str = vConfig.Scale.ReleasedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = vConfig.Scale.Released.x.ToString();
                FieldGet((id + 2).ToString())?.Str = vConfig.Scale.Released.y.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref vConfig.Scale.PressedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref vConfig.Scale.PressedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(1.0f, 0.68f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_LeftRight, "X", ref vConfig.Scale.Pressed.x, 0, 10f, 300f);
            GUI.color = new Color(0.68f, 1.0f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_UpDown, "Y", ref vConfig.Scale.Pressed.y, 0, 10f, 300f);
            GUILayout.BeginHorizontal();
            GUI.color = old;
            GUILayout.Label(Drawer.Icon_Up);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                vConfig.Scale.Released = vConfig.Scale.Pressed;
                vConfig.Scale.ReleasedEase = vConfig.Scale.PressedEase.Copy();
                FieldGet(id.ToString())?.Str = vConfig.Scale.PressedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = vConfig.Scale.Pressed.x.ToString();
                FieldGet((id + 2).ToString())?.Str = vConfig.Scale.Pressed.y.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref vConfig.Scale.ReleasedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref vConfig.Scale.ReleasedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(1.0f, 0.68f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_LeftRight, "X", ref vConfig.Scale.Released.x, 0, 10f, 300f);
            GUI.color = new Color(0.68f, 1.0f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_UpDown, "Y", ref vConfig.Scale.Released.y, 0, 10f, 300f);
            GUI.color = old;

            int screenhw = Screen.width / 2;
            int screenhh = Screen.height / 2;
            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Offset);
            GUILayout.Label($"<b>{Main.Lang.Get("OFFSET", "Offset")}</b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUI.color = old;
            GUILayout.Label(Drawer.Icon_Down);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                vConfig.Offset.Pressed = vConfig.Offset.Released;
                vConfig.Offset.PressedEase = vConfig.Offset.ReleasedEase.Copy();
                FieldGet(id.ToString())?.Str = vConfig.Offset.PressedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = vConfig.Offset.Pressed.x.ToString();
                FieldGet((id + 2).ToString())?.Str = vConfig.Offset.Pressed.y.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref vConfig.Offset.PressedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref vConfig.Offset.PressedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(1.0f, 0.68f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_LeftRight, "X", ref vConfig.Offset.Pressed.x, -screenhw, screenhw, 300f);
            GUI.color = new Color(0.68f, 1.0f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_UpDown, "Y", ref vConfig.Offset.Pressed.y, -screenhh, screenhh, 300f);
            GUILayout.BeginHorizontal();
            GUI.color = old;
            GUILayout.Label(Drawer.Icon_Up);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                vConfig.Offset.Released = vConfig.Offset.Pressed;
                vConfig.Offset.ReleasedEase = vConfig.Offset.PressedEase.Copy();
                FieldGet(id.ToString())?.Str = vConfig.Offset.PressedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = vConfig.Offset.Pressed.x.ToString();
                FieldGet((id + 2).ToString())?.Str = vConfig.Offset.Pressed.y.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref vConfig.Offset.ReleasedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref vConfig.Offset.ReleasedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(1.0f, 0.68f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_LeftRight, "X", ref vConfig.Offset.Released.x, -screenhw, screenhw, 300f);
            GUI.color = new Color(0.68f, 1.0f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_UpDown, "Y", ref vConfig.Offset.Released.y, -screenhh, screenhh, 300f);
            GUI.color = old;

            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Rotate);
            GUILayout.Label($"<b>{Main.Lang.Get("ROTATION", "Rotation")}</b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUI.color = old;
            GUILayout.Label(Drawer.Icon_Down);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                vConfig.Rotation.Pressed = vConfig.Rotation.Released;
                vConfig.Rotation.PressedEase = vConfig.Rotation.ReleasedEase.Copy();
                FieldGet(id.ToString())?.Str = vConfig.Rotation.ReleasedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = vConfig.Rotation.Released.x.ToString();
                FieldGet((id + 2).ToString())?.Str = vConfig.Rotation.Released.y.ToString();
                FieldGet((id + 3).ToString())?.Str = vConfig.Rotation.Released.z.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref vConfig.Rotation.PressedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref vConfig.Rotation.PressedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(1.0f, 0.68f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_XRotate, "X", ref vConfig.Rotation.Pressed.x, -180f, 180f, 300f);
            GUI.color = new Color(0.68f, 1.0f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_YRotate, "Y", ref vConfig.Rotation.Pressed.y, -180f, 180f, 300f);
            GUI.color = new Color(0.68f, 0.68f, 1.0f);
            changed |= DrawSingleWithSlider(Drawer.Icon_ZRotate, "Z", ref vConfig.Rotation.Pressed.z, -180f, 180f, 300f);
            GUILayout.BeginHorizontal();
            GUI.color = old;
            GUILayout.Label(Drawer.Icon_Up);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                vConfig.Rotation.Released = vConfig.Rotation.Pressed;
                vConfig.Rotation.ReleasedEase = vConfig.Rotation.PressedEase.Copy();
                FieldGet(id.ToString())?.Str = vConfig.Rotation.PressedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = vConfig.Rotation.Pressed.x.ToString();
                FieldGet((id + 2).ToString())?.Str = vConfig.Rotation.Pressed.y.ToString();
                FieldGet((id + 3).ToString())?.Str = vConfig.Rotation.Pressed.z.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref vConfig.Rotation.ReleasedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref vConfig.Rotation.ReleasedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(1.0f, 0.68f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_XRotate, "X", ref vConfig.Rotation.Released.x, -180f, 180f, 300f);
            GUI.color = new Color(0.68f, 1.0f, 0.68f);
            changed |= DrawSingleWithSlider(Drawer.Icon_YRotate, "Y", ref vConfig.Rotation.Released.y, -180f, 180f, 300f);
            GUI.color = new Color(0.68f, 0.68f, 1.0f);
            changed |= DrawSingleWithSlider(Drawer.Icon_ZRotate, "Z", ref vConfig.Rotation.Released.z, -180f, 180f, 300f);
            GUI.color = old;

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(6);
            GUILayout.BeginVertical();
            changed |= Drawer.DrawPivot(ref vConfig.Pivot);
            GUILayout.EndVertical();
            GUILayout.Space(16);
            GUILayout.BeginVertical();
            changed |= Drawer.DrawAnchor(ref vConfig.Anchor);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return changed;
        }

        public bool DrawObjectConfig(ObjectConfig oConfig) {
            bool changed = false;
            Color old = GUI.color;
            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Down);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                oConfig.Color.Released = oConfig.Color.Pressed;
                oConfig.Color.ReleasedEase = oConfig.Color.PressedEase.Copy();
                FieldGet(id.ToString())?.Str = oConfig.Color.PressedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = oConfig.Color.Pressed.topLeftHex;
                FieldGet((id + 2).ToString())?.Str = oConfig.Color.Pressed.topRightHex;
                FieldGet((id + 3).ToString())?.Str = oConfig.Color.Pressed.bottomLeftHex;
                FieldGet((id + 4).ToString())?.Str = oConfig.Color.Pressed.bottomRightHex;
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref oConfig.Color.ReleasedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref oConfig.Color.ReleasedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            DrawGColor(ref oConfig.Color.Released, true);
            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Up);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                oConfig.Color.Pressed = oConfig.Color.Released;
                oConfig.Color.PressedEase = oConfig.Color.ReleasedEase.Copy();
                FieldGet(id.ToString())?.Str = oConfig.Color.ReleasedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = oConfig.Color.Released.topLeftHex;
                FieldGet((id + 2).ToString())?.Str = oConfig.Color.Released.topRightHex;
                FieldGet((id + 3).ToString())?.Str = oConfig.Color.Released.bottomLeftHex;
                FieldGet((id + 4).ToString())?.Str = oConfig.Color.Released.bottomRightHex;
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref oConfig.Color.PressedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref oConfig.Color.PressedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Center);
            GUILayout.Label($"<b>{Main.Lang.Get("COLOR", "Color")}</b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            DrawGColor(ref oConfig.Color.Pressed, true);
            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Center);
            GUILayout.Label($"<b>{Main.Lang.Get("VECTOR", "Vector")}</b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            DrawVectorConfig(oConfig.VectorConfig);

            return changed;
        }

        public bool DrawRainConfig(RainConfig rConfig) {
            bool changed = false;
            Color old = GUI.color;

            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Scale);
            GUILayout.Label($"<b>{Main.Lang.Get("SPEED", "Speed")}</b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Down);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                rConfig.Speed.Pressed = rConfig.Speed.Released;
                rConfig.Speed.PressedEase = rConfig.Speed.ReleasedEase.Copy();
                FieldGet(id.ToString())?.Str = rConfig.Speed.ReleasedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = rConfig.Speed.Released.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref rConfig.Speed.PressedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref rConfig.Speed.PressedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(0.44f, 1f, 0.92f);
            changed |= DrawSingleWithSlider(Drawer.Icon_Speed, "V", ref rConfig.Speed.Pressed, 0, 500f, 300f);
            GUILayout.BeginHorizontal();
            GUI.color = old;
            GUILayout.Label(Drawer.Icon_Up);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                rConfig.Speed.Released = rConfig.Speed.Pressed;
                rConfig.Speed.ReleasedEase = rConfig.Speed.PressedEase.Copy();
                FieldGet(id.ToString())?.Str = rConfig.Speed.PressedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = rConfig.Speed.Pressed.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref rConfig.Speed.ReleasedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref rConfig.Speed.ReleasedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(0.44f, 1f, 0.92f);
            changed |= DrawSingleWithSlider(Drawer.Icon_Speed, "V", ref rConfig.Speed.Released, 0, 500f, 300f);
            GUI.color = old;

            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Scale);
            GUILayout.Label($"<b>{Main.Lang.Get("LENGTH", "Length")}</b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Down);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                rConfig.Length.Pressed = rConfig.Length.Released;
                rConfig.Length.PressedEase = rConfig.Length.ReleasedEase.Copy();
                FieldGet(id.ToString())?.Str = rConfig.Length.ReleasedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = rConfig.Length.Released.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref rConfig.Length.PressedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref rConfig.Length.PressedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(0.63f, 0.44f, 1f);
            changed |= DrawSingleWithSlider(Drawer.Icon_LeftRight, "L", ref rConfig.Length.Pressed, 0, 500f, 300f);
            GUILayout.BeginHorizontal();
            GUI.color = old;
            GUILayout.Label(Drawer.Icon_Up);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                rConfig.Length.Released = rConfig.Length.Pressed;
                rConfig.Length.ReleasedEase = rConfig.Length.PressedEase.Copy();
                FieldGet(id.ToString())?.Str = rConfig.Length.PressedEase.Duration.ToString();
                FieldGet((id + 1).ToString())?.Str = rConfig.Length.Pressed.ToString();
                changed = true;
            }
            GUI.color = old;
            Drawer.DrawEase(ref rConfig.Length.ReleasedEase.Ease);
            GUILayout.Space(7);
            changed |= DrawSingleWithSlider(Drawer.Icon_Duration, "", ref rConfig.Length.ReleasedEase.Duration, 0, 5f, 170f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = new Color(0.63f, 0.44f, 1f);
            changed |= DrawSingleWithSlider(Drawer.Icon_LeftRight, "L", ref rConfig.Length.Released, 0, 500f, 300f);
            GUI.color = old;

            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Scale);
            GUILayout.Label($"<b>{Main.Lang.Get("SOFTNESS", "Softness")}</b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Drawer.Icon_Down);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                rConfig.Softness.Pressed = rConfig.Softness.Released;
                FieldGet(id.ToString())?.Str = rConfig.Softness.Released.ToString();
                changed = true;
            }
            GUI.color = new Color(0.78f, 1f, 0.44f);
            changed |= DrawInt32WithSlider(Drawer.Icon_LeftRight, "S", ref rConfig.Softness.Pressed, 0, 1000, 300f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUI.color = old;
            GUILayout.Label(Drawer.Icon_Up);
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if(Drawer.Button(Drawer.Icon_Copy, GUILayout.Width(34))) {
                rConfig.Softness.Released = rConfig.Softness.Pressed;
                FieldGet(id.ToString())?.Str = rConfig.Softness.Pressed.ToString();
                changed = true;
            }
            GUI.color = new Color(0.78f, 1f, 0.44f);
            changed |= DrawInt32WithSlider(Drawer.Icon_LeftRight, "S", ref rConfig.Softness.Released, 0, 1000, 300f);
            GUI.color = old;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            DrawObjectConfig(rConfig.ObjectConfig);

            return changed;
        }
    }
}