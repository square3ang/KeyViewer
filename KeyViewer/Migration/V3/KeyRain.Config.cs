using System.Xml.Serialization;
using UnityEngine;

namespace KeyViewer.Migration.V3
{
    [XmlRoot("RainConfig")]
    public class KeyRain_Config
    {
        public float OffsetX = 0f;
        public float OffsetY = 0f;
        public float RainSpeed = 400f;
        public float RainWidth = -1f;
        public float RainHeight = -1f;
        public float RainLength = 400f;
        public int Softness = 100;
        public Color RainColor = Color.white;
        public string[] RainImages = new string[0];
        public int RainPoolSize = 25;
        public int[] RainImageCounts = new int[0];
        public bool SequentialImages = true;
        public bool ShuffleImages;
        public Direction Direction = Direction.U;

        [XmlIgnore]
        public bool ColorExpanded = false;
    }
}
