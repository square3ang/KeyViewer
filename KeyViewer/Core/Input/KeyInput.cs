using UnityEngine;
using SyncInput = UnityEngine.Input;

namespace KeyViewer.Core.Input
{
    public static class KeyInput
    {
        public static bool AsyncAvailable => false; //AsyncInputManager.isActive;
        public static bool AnyKey => AsyncAvailable ? AsyncInputCompat.AnyKey : SyncInput.anyKey;
        public static bool AnyKeyDown => AsyncAvailable ? AsyncInputCompat.AnyKeyDown : SyncInput.anyKeyDown;
        public static bool Shift => GetKey(KeyCode.LeftShift) || GetKey(KeyCode.RightShift);
        public static bool Control => GetKey(KeyCode.LeftControl) || GetKey(KeyCode.RightControl);
        public static bool Alt => GetKey(KeyCode.LeftAlt) || GetKey(KeyCode.RightAlt);
        public static bool GetKey(KeyCode code)
        {
            if (Main.IsWindows && WinInput.TryGetState(code, out bool state))
                return state;
            if (AsyncAvailable)
                return AsyncInputCompat.GetKey(code);
            return SyncInput.GetKey(code);
        }
        public static bool GetKeyUp(KeyCode code)
        {
            if (AsyncAvailable)
                return AsyncInputCompat.GetKeyUp(code);
            return SyncInput.GetKeyUp(code);
        }
        public static bool GetKeyDown(KeyCode code)
        {
            if (AsyncAvailable)
                return AsyncInputCompat.GetKeyDown(code);
            return SyncInput.GetKeyDown(code);
        }
    }
}
