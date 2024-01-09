using KeyViewer.API;
using UnityEngine;

namespace KeyViewer
{
    public static class ReplayInput
    {
        public static void OnStartInputs() => InputAPI.Active = true;
        public static void OnEndInputs() => InputAPI.Active = false;
        public static void OnKeyPressed(KeyCode key) => InputAPI.PressKey(key);
        public static void OnKeyReleased(KeyCode key) => InputAPI.ReleaseKey(key);
    }
}
