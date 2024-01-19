using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Migration.V2
{
    public class KeyViewerProfile
    {
        public string Name { get; set; }
        public List<KeyCode> ActiveKeys { get; set; } = new List<KeyCode>();
        public bool ViewerOnlyGameplay { get; set; }
        public bool AnimateKeys { get; set; } = true;
        public bool ShowKeyPressTotal { get; set; } = true;
        public float KeyViewerSize { get; set; } = 100f;
        public float KeyViewerXPos { get; set; } = 0.89f;
        public float KeyViewerYPos { get; set; } = 0.03f;
        public Color PressedOutlineColor;
        public Color ReleasedOutlineColor;
        public Color PressedBackgroundColor;
        public Color ReleasedBackgroundColor;
        public Color PressedTextColor;
        public Color ReleasedTextColor;
        public KeyViewerProfile()
        {
            this.PressedOutlineColor = Color.white;
            this.ReleasedOutlineColor = Color.white;
            this.PressedBackgroundColor = Color.white;
            this.ReleasedBackgroundColor = Color.black.WithAlpha(0.4f);
            this.PressedTextColor = Color.black;
            this.ReleasedTextColor = Color.white;
        }
    }
}
