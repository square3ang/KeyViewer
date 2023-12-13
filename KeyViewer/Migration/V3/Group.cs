using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Migration.V3
{
    public class Group
    {
        public Group() { }
        public List<KeyCode> codes = new List<KeyCode>();
        public Key_Config groupConfig;
        public string Name = "Group";
        public bool Editing = false;
    }
}
