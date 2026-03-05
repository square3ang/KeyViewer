using System;
using UnityEngine;

namespace RapidGUI;

public class RapidGUIBehaviour : MonoBehaviour {
    #region static 

    public static RapidGUIBehaviour Instance {
        get {
            if(field == null) {
                field = FindObjectOfType<RapidGUIBehaviour>();
                if(field == null) {
                    var ga = new GameObject("RapidGUI");
                    field = ga.AddComponent<RapidGUIBehaviour>();
                }

                if(Application.isPlaying) {
                    DontDestroyOnLoad(field);
                }
            }

            return field;
        }
    }

    #endregion

    public KeyCode closeFocusedWindowKey = KeyCode.Q;
    public int prefixLabelSlideButton = 1;
    public Action onGUI;

    public void OnGUI() => onGUI?.Invoke();
}