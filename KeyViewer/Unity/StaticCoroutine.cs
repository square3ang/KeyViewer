using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Unity
{
    public class StaticCoroutine : MonoBehaviour
    {
        static StaticCoroutine Runner
        {
            get
            {
                if (!runner)
                {
                    runner = new GameObject().AddComponent<StaticCoroutine>();
                    DontDestroyOnLoad(runner.gameObject);
                    return runner;
                }
                return runner;
            }
        }
        static StaticCoroutine runner;
        static Queue<IEnumerator> routines = new Queue<IEnumerator>();
        /// <summary>
        /// Run Coroutine Immediately
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public static Coroutine Run(IEnumerator coroutine) => Runner.StartCoroutine(coroutine);
        /// <summary>
        /// Queue Coroutine At Next Frame
        /// </summary>
        /// <param name="coroutine"></param>
        public static void Queue(IEnumerator coroutine) => routines.Enqueue(coroutine);
        /// <summary>
        /// Run Action On Coroutine
        /// </summary>
        /// <param name="sync"></param>
        public static void RAct(Action sync) => Run(SyncRunner(sync));
        /// <summary>
        /// Queue Action On Coroutine
        /// </summary>
        /// <param name="sync"></param>
        public static void QAct(Action sync) => Queue(SyncRunner(sync));
        public static IEnumerator SyncRunner(Action routine, object firstYield = null)
        {
            yield return firstYield;
            routine?.Invoke();
            yield break;
        }
        void Update()
        {
            while (routines.Count > 0)
                StartCoroutine(routines.Dequeue());
        }
    }
}
