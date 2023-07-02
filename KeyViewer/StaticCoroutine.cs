using System.Collections;
using UnityEngine;

namespace KeyViewer
{
    public class StaticCoroutine : MonoBehaviour
    {
        public static StaticCoroutine Instance
        {
            get
            {
                if (!instance)
                {
                    instance = new GameObject("KeyViewer_StaticCoroutine").AddComponent<StaticCoroutine>();
                    return instance;
                }
                return instance;
            }
        }
        private static StaticCoroutine instance;
        public static Coroutine Run(IEnumerator coroutine) => Instance.StartCoroutine(coroutine);
    }
}
