using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KeyViewer
{
    public static class KeyInput
    {
        public static bool GetKey(KeyCode code)
        {
            if (AsyncInputManager.isActive)
                return AsyncInputCompat.GetKey(code);
            return Input.GetKey(code);
        }
        public static bool GetKeyUp(KeyCode code)
        {
            if (AsyncInputManager.isActive)
                return AsyncInputCompat.GetKeyUp(code);
            return Input.GetKeyUp(code);
        }
        public static bool GetKeyDown(KeyCode code)
        {
            if (AsyncInputManager.isActive)
                return AsyncInputCompat.GetKeyDown(code);
            return Input.GetKeyDown(code);
        }
    }
}
