using System.Xml.Serialization;
using UnityEngine;

namespace KeyViewer.Migration.V3
{
    public class KeyRain_Config
    {
        public float OffsetX = 0f;
        public float OffsetY = 0f;
        public float RainSpeed = 400f;
        public float RainWidth = -1f;
        public float RainHeight = -1f;
        public float RainLength = 400f;
        public int RainPoolSize = 25;
        public int Softness = 100;
        public Color RainColor = Color.white;
        public string RainImage = null;
        public Direction Direction = Direction.U;

        [XmlIgnore]
        public bool ColorExpanded = false;

        public KeyRain_Config Copy()
        {
            KeyRain_Config newConfig = new KeyRain_Config();
            newConfig.OffsetX = OffsetX;
            newConfig.OffsetY = OffsetY;
            newConfig.RainSpeed = RainSpeed;
            newConfig.RainWidth = RainWidth;
            newConfig.RainHeight = RainHeight;
            newConfig.RainLength = RainLength;
            newConfig.RainColor = RainColor;
            newConfig.RainImage = RainImage;
            newConfig.Softness = Softness;
            newConfig.Direction = Direction;
            return newConfig;
        }
    }
}
