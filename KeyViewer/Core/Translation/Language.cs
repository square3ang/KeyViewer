using GDMiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using KeyViewer.Unity;
using UnityEngine;

namespace KeyViewer.Core.Translation
{
    public class Language
    {
        public static string GetUrl(int gid) =>
            $"https://docs.google.com/spreadsheets/d/1wG6wB3q0q1E647mhECPSl5Sd_joqlsYOmkoPyDMs-Rw/edit#gid={gid}";
        private static Language Korean;
        private static Language English;
        public readonly GID gid;
        public readonly string url;
        public readonly Dictionary<string, string> dict;
        public bool Initialized { get; private set; }
        public Language(GID gid)
        {
            this.gid = gid;
            url = GetUrl((int)gid);
            dict = new Dictionary<string, string>();
            StaticCoroutine.Run(Download());
        }
        public string this[string key]
        {
            get => dict.TryGetValue(key, out string value) ? value : key;
            set => dict[key] = value;
        }
        public static Language GetLanguage(SystemLanguage lang)
        {
            switch (lang)
            {
                case SystemLanguage.English:
                    return English ??= new Language(GID.ENGLISH);
                case SystemLanguage.Korean:
                    return Korean ??= new Language(GID.KOREAN);
                default: return English;
            }
        }
        public static void Release()
        {
            Korean = null;
            English = null;
        }
        IEnumerator Download()
        {
            if (Initialized) yield break;
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            byte[] bytes = request.downloadHandler.data;
            if (bytes == null)
            {
                Initialized = true;
                yield break;
            }
            string strData = Encoding.UTF8.GetString(bytes);
            strData = strData.Substring(47, strData.Length - 49);
            StringBuilder sb = new StringBuilder();
            foreach (object obj in ((Json.Deserialize(strData) as Dictionary<string, object>)["table"] as Dictionary<string, object>)["rows"] as List<object>)
            {
                List<object> list = (obj as Dictionary<string, object>)["c"] as List<object>;
                string key = (list[0] as Dictionary<string, object>)["v"] as string;
                string value = (list[1] as Dictionary<string, object>)["v"] as string;
                if (key.IsNullOrEmpty() || value.IsNullOrEmpty())
                    continue;
                dict.Add(key, value);
                sb.AppendLine(Escape(key) + ":" + Escape(value));
            }
            Main.Logger.Log($"Loaded {dict.Count} Localizations from Sheet");
            Initialized = true;
        }
        static string Escape(string str) => str.Replace(@"\", @"\\").Replace(":", @"\:");
    }
}
