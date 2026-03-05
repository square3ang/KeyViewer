using UnityEngine;
using SyncInput = UnityEngine.Input;

namespace KeyViewer.Core.Input;

public static class KeyInput {
    public static bool AsyncAvailable => false; //AsyncInputManager.isActive;
    public static bool AnyKey => AsyncAvailable ? AsyncInputCompat.AnyKey : SyncInput.anyKey;
    public static bool AnyKeyDown => AsyncAvailable ? AsyncInputCompat.AnyKeyDown : SyncInput.anyKeyDown;
    public static bool Shift => GetKey(KeyCode.LeftShift) || GetKey(KeyCode.RightShift);
    public static bool Control => GetKey(KeyCode.LeftControl) || GetKey(KeyCode.RightControl);
    public static bool Alt => GetKey(KeyCode.LeftAlt) || GetKey(KeyCode.RightAlt);
    public static bool GetKey(KeyCode code) {
        return Main.IsWindows && WinInput.TryGetState(code, out bool state)
            ? state
            : AsyncAvailable ? AsyncInputCompat.GetKey(code) : SyncInput.GetKey(code);
    }
    public static bool GetKeyUp(KeyCode code) => AsyncAvailable ? AsyncInputCompat.GetKeyUp(code) : SyncInput.GetKeyUp(code);
    public static bool GetKeyDown(KeyCode code) => AsyncAvailable ? AsyncInputCompat.GetKeyDown(code) : SyncInput.GetKeyDown(code);
}
