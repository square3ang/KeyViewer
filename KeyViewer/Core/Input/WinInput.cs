using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace KeyViewer.Core.Input;

public enum PollingRate {
    HzMonitor = -1,
    Hz250 = 250,
    Hz500 = 500,
    Hz1000 = 1000,
    Hz2000 = 2000,
    Hz4000 = 4000
}

public static class WinInput {
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    public static event Action<int> OnKeyDown;
    public static event Action<int> OnKeyUp;

    private static readonly bool[] keyStates = new bool[0xFE];
    private static readonly bool[] prevStates = new bool[0xFE];

    private static Thread pollingThread;
    private static bool running = false;
    public static bool IsPolling => running;

    public static void StartPolling(PollingRate rate) {
        if(!Main.IsWindows || running) {
            return;
        }

        running = true;
        long targetTicks = Stopwatch.Frequency / (rate == PollingRate.HzMonitor ? Application.targetFrameRate : (int)rate);

        pollingThread = new Thread(() =>
        {
            var sw = Stopwatch.StartNew();
            long previousTick = sw.ElapsedTicks;

            while(running) {
                UpdateStates();

                while(sw.ElapsedTicks - previousTick < targetTicks) {
                    if(targetTicks - (sw.ElapsedTicks - previousTick) > (long)(Stopwatch.Frequency * 0.002)) {
                        Thread.Sleep(1);
                    }
                }

                previousTick = sw.ElapsedTicks;
            }
        }) {
            IsBackground = true
        };
        pollingThread.Start();
    }

    public static void StopPolling() {
        running = false;

        if(pollingThread != null && pollingThread.IsAlive) {
            if(!pollingThread.Join(1000)) {
                pollingThread.Abort();
            }
        }

        pollingThread = null;
    }

    public static void SetPollingRate(PollingRate rate) {
        if(!Main.IsWindows || !running) {
            return;
        }
        StopPolling();
        StartPolling(rate);
    }

    private static void UpdateStates() {
        for(int vk = 0x01; vk < 0xFF; vk++) {
            bool prev = prevStates[vk-1];
            bool curr = (GetAsyncKeyState(vk) & 0x8000) != 0;

            keyStates[vk - 1] = curr;

            if(curr != prev) {
                //int vkCopy = vk;
                if(curr) {
                    OnKeyDown?.Invoke(vk);
                    //Main.MainThreadDispatcher.Enqueue(() => Main.Logger.Log($"Key Down: {vkCopy:X2}"));
                } else {
                    OnKeyUp?.Invoke(vk);
                    //Main.MainThreadDispatcher.Enqueue(() => Main.Logger.Log($"Key Up: {vkCopy:X2}"));
                }
            }

            prevStates[vk - 1] = curr;
        }
    }

    public static bool TryGetKeyState(int vk, out bool state) {
        if(vk > 0 && vk < keyStates.Length) {
            state = keyStates[vk - 1];
            return true;
        }
        state = false;
        return false;
    }

    public static bool AnyKey() {
        for(int i = 0; i < keyStates.Length; i++) {
            if(keyStates[i]) {
                return true;
            }
        }
        return false;
    }

    public static bool AnyKeyDown() {
        for(int i = 0; i < keyStates.Length; i++) {
            if(keyStates[i] && !prevStates[i]) {
                return true;
            }
        }
        return false;
    }

    public static void Dispose() {
        StopPolling();
        Array.Clear(keyStates, 0, keyStates.Length);
        Array.Clear(prevStates, 0, prevStates.Length);
        OnKeyDown = null;
        OnKeyUp = null;
    }
    public static KeyCode IntToKeyCode(int vk) {
        foreach(var pair in keyTable) {
            if(pair.Value == vk) {
                return pair.Key;
            }
        }
        return KeyCode.None;
    }

    public static List<int> KeyCodeToInts(KeyCode code) => [.. keyTable.Where(pair => pair.Key == code).Select(pair => pair.Value)];

    private static readonly KeyValuePair<KeyCode, int>[] keyTable = [
        new(KeyCode.Mouse0, 0x01),
        new(KeyCode.Mouse1, 0x02),
        new(KeyCode.Mouse2, 0x04),
        new(KeyCode.Mouse3, 0x05),
        new(KeyCode.Mouse4, 0x06),

        new(KeyCode.Alpha0, 0x30),
        new(KeyCode.Alpha1, 0x31),
        new(KeyCode.Alpha2, 0x32),
        new(KeyCode.Alpha3, 0x33),
        new(KeyCode.Alpha4, 0x34),
        new(KeyCode.Alpha5, 0x35),
        new(KeyCode.Alpha6, 0x36),
        new(KeyCode.Alpha7, 0x37),
        new(KeyCode.Alpha8, 0x38),
        new(KeyCode.Alpha9, 0x39),

        new(KeyCode.A, 0x41),
        new(KeyCode.B, 0x42),
        new(KeyCode.C, 0x43),
        new(KeyCode.D, 0x44),
        new(KeyCode.E, 0x45),
        new(KeyCode.F, 0x46),
        new(KeyCode.G, 0x47),
        new(KeyCode.H, 0x48),
        new(KeyCode.I, 0x49),
        new(KeyCode.J, 0x4A),
        new(KeyCode.K, 0x4B),
        new(KeyCode.L, 0x4C),
        new(KeyCode.M, 0x4D),
        new(KeyCode.N, 0x4E),
        new(KeyCode.O, 0x4F),
        new(KeyCode.P, 0x50),
        new(KeyCode.Q, 0x51),
        new(KeyCode.R, 0x52),
        new(KeyCode.S, 0x53),
        new(KeyCode.T, 0x54),
        new(KeyCode.U, 0x55),
        new(KeyCode.V, 0x56),
        new(KeyCode.W, 0x57),
        new(KeyCode.X, 0x58),
        new(KeyCode.Y, 0x59),
        new(KeyCode.Z, 0x5A),

        new(KeyCode.LeftArrow, 0x25),
        new(KeyCode.UpArrow, 0x26),
        new(KeyCode.RightArrow, 0x27),
        new(KeyCode.DownArrow, 0x28),
        new(KeyCode.Backspace, 0x08),
        new(KeyCode.Tab, 0x09),
        new(KeyCode.Clear, 0x0C),
        new(KeyCode.Return, 0x0D),
        new(KeyCode.Pause, 0x13),
        new(KeyCode.CapsLock, 0x14),
        new(KeyCode.Escape, 0x1B),
        new(KeyCode.Space, 0x20),
        new(KeyCode.Slash, 0xBF),
        new(KeyCode.Backslash, 0xDC),
        new(KeyCode.BackQuote, 0xC0),
        new(KeyCode.Minus, 0xBD),
        new(KeyCode.Equals, 0xBB),
        new(KeyCode.LeftBracket, 0xDB),
        new(KeyCode.RightBracket, 0xDD),
        new(KeyCode.Semicolon, 0xBA),
        new(KeyCode.Quote, 0xDE),
        new(KeyCode.Comma, 0xBC),
        new(KeyCode.Period, 0xBE),
        new(KeyCode.PageUp, 0x21),
        new(KeyCode.PageDown, 0x22),
        new(KeyCode.End, 0x23),
        new(KeyCode.Home, 0x24),
        new(KeyCode.Insert, 0x2D),
        new(KeyCode.Delete, 0x2E),
        new(KeyCode.Print, 0x9A),
        new(KeyCode.LeftWindows, 0x5B),
        new(KeyCode.RightWindows, 0x5C),
        new(KeyCode.Numlock, 0x90),
        new(KeyCode.ScrollLock, 0x91),
        new(KeyCode.LeftShift, 0xA0),
        new(KeyCode.RightShift, 0xA1),
        new(KeyCode.LeftControl, 0xA2),
        new(KeyCode.LeftAlt, 0xA4),
        new(KeyCode.RightControl, 0xA3),
        new(KeyCode.RightControl, 0x19), // Hangul
        new(KeyCode.RightAlt, 0xA5),
        new(KeyCode.RightAlt, 0x15), // Hangul
        new(KeyCode.Menu, 0x5D),

        new(KeyCode.F1, 0x70),
        new(KeyCode.F2, 0x71),
        new(KeyCode.F3, 0x72),
        new(KeyCode.F4, 0x73),
        new(KeyCode.F5, 0x74),
        new(KeyCode.F6, 0x75),
        new(KeyCode.F7, 0x76),
        new(KeyCode.F8, 0x77),
        new(KeyCode.F9, 0x78),
        new(KeyCode.F10, 0x79),
        new(KeyCode.F11, 0x7A),
        new(KeyCode.F12, 0x7B),
        new(KeyCode.F13, 0x7C),
        new(KeyCode.F14, 0x7D),
        new(KeyCode.F15, 0x7E),
        new(KeyCode.F16, 0x7F),
        new(KeyCode.F17, 0x80),
        new(KeyCode.F18, 0x81),
        new(KeyCode.F19, 0x82),
        new(KeyCode.F20, 0x83),
        new(KeyCode.F21, 0x84),
        new(KeyCode.F22, 0x85),
        new(KeyCode.F23, 0x86),
        new(KeyCode.F24, 0x87),

        new(KeyCode.Keypad0, 0x60),
        new(KeyCode.Keypad1, 0x61),
        new(KeyCode.Keypad2, 0x62),
        new(KeyCode.Keypad3, 0x63),
        new(KeyCode.Keypad4, 0x64),
        new(KeyCode.Keypad5, 0x65),
        new(KeyCode.Keypad6, 0x66),
        new(KeyCode.Keypad7, 0x67),
        new(KeyCode.Keypad8, 0x68),
        new(KeyCode.Keypad9, 0x69),
        new(KeyCode.KeypadMultiply, 0x6A),
        new(KeyCode.KeypadPlus, 0x6B),
        new(KeyCode.KeypadEnter, 0x6C),
        new(KeyCode.KeypadMinus, 0x6D),
        new(KeyCode.KeypadPeriod, 0x6E),
        new(KeyCode.KeypadDivide, 0x6F),

        new(KeyCode.JoystickButton0, 0xC3),
        new(KeyCode.JoystickButton1, 0xC4),
        new(KeyCode.JoystickButton2, 0xC5),
        new(KeyCode.JoystickButton3, 0xC6),
        new(KeyCode.JoystickButton4, 0xC8),
        new(KeyCode.JoystickButton5, 0xC7),
        new(KeyCode.JoystickButton6, 0xC9),
        new(KeyCode.JoystickButton7, 0xCA),
        new(KeyCode.JoystickButton8, 0xCF),
        new(KeyCode.JoystickButton9, 0xD0),
        new(KeyCode.JoystickButton10, 0xD1),
        new(KeyCode.JoystickButton11, 0xD2),
        new(KeyCode.JoystickButton12, 0xCB),
        new(KeyCode.JoystickButton13, 0xCC),
        new(KeyCode.JoystickButton14, 0xCD),
        new(KeyCode.JoystickButton15, 0xCE),
    ];
}