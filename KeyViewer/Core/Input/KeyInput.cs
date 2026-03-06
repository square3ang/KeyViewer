using UnityEngine;
using SyncInput = UnityEngine.Input;

namespace KeyViewer.Core.Input;

public static class KeyInput {
    public static bool AnyKey => (Main.IsWindows && Main.Settings.UseWindowsAsyncInput && Application.isFocused)
        ? WinInput.AnyKey() : SyncInput.anyKey;

    public static bool AnyKeyDown => (Main.IsWindows && Main.Settings.UseWindowsAsyncInput && Application.isFocused)
        ? WinInput.AnyKeyDown() : SyncInput.anyKey;

    public static bool Shift => GetKey(KeyCode.LeftShift) || GetKey(KeyCode.RightShift);
    public static bool Control => GetKey(KeyCode.LeftControl) || GetKey(KeyCode.RightControl);
    public static bool Alt => GetKey(KeyCode.LeftAlt) || GetKey(KeyCode.RightAlt);

    public static bool GetKey(KeyCode code) {
        if(Main.IsWindows && Main.Settings.UseWindowsAsyncInput && Application.isFocused) {
            int vk = WinInput.KeyCodeToInt(code);
            if(vk != 0) {
                return WinInput.TryGetKeyState(vk, out bool state) && state;
            }
            return false;
        }

        return SyncInput.GetKey(code);
    }

    public static bool GetKeyDown(KeyCode code) {
        return SyncInput.GetKeyDown(code);
    }

    public static bool GetKeyUp(KeyCode code) {
        return SyncInput.GetKeyUp(code);
    }
}