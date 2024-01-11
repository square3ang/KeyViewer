using UnityEngine;
using SyncInput = UnityEngine.Input;

namespace KeyViewer.Core.Input
{
    public static class KeyInput
    {
        public static bool AnyKey => AsyncInputManager.isActive && scrController.instance.currentState == States.PlayerControl ? AsyncInputCompat.AnyKey : SyncInput.anyKey;
        public static bool AnyKeyDown => AsyncInputManager.isActive && scrController.instance.currentState == States.PlayerControl ? AsyncInputCompat.AnyKeyDown : SyncInput.anyKeyDown;
        public static bool GetKey(KeyCode code)
        {
            if (AsyncInputManager.isActive && scrController.instance.gameworld && scrController.instance.currentState == States.PlayerControl)
                return AsyncInputCompat.GetKey(code);
            return SyncInput.GetKey(code);
        }
        public static bool GetKeyUp(KeyCode code)
        {
            if (AsyncInputManager.isActive && scrController.instance.gameworld && scrController.instance.currentState == States.PlayerControl)
                return AsyncInputCompat.GetKeyUp(code);
            return SyncInput.GetKeyUp(code);
        }
        public static bool GetKeyDown(KeyCode code)
        {
            if (AsyncInputManager.isActive && scrController.instance.gameworld && scrController.instance.currentState == States.PlayerControl)
                return AsyncInputCompat.GetKeyDown(code);
            return SyncInput.GetKeyDown(code);
        }
    }
}
