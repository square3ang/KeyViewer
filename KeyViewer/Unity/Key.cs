using KeyViewer.Models;
using UnityEngine;

namespace KeyViewer.Unity
{
    public class Key : MonoBehaviour
    {
        public KeyManager manager;
        public KeyConfig config;
        public void Init(KeyManager manager, KeyConfig config)
        {
            this.manager = manager;
            this.config = config;
        }
        public void UpdateLayout(ref float x, ref float tempX, int updateCount)
        {

        }
    }
}
