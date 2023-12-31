using JSON;
using KeyViewer.Models;
using KeyViewer.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace KeyViewer.Core.Translation
{
    public class Language
    {
        private const string SPREADSHEET_URL_START = "https://docs.google.com/spreadsheets/d/";
        private const string SPREADSHEET_URL_END = "/gviz/tq?tqx=out:json&tq&gid=";
        private const string SPREADSHEET_URL_KEY = "1wG6wB3q0q1E647mhECPSl5Sd_joqlsYOmkoPyDMs-Rw";
        private static string GetUrl(GID gid) => SPREADSHEET_URL_START + SPREADSHEET_URL_KEY + SPREADSHEET_URL_END + (int)gid;
        private static Language Korean;
        private static Language English;
        public readonly GID gid;
        public readonly string url;
        public readonly Dictionary<string, string> dict;
        private event Action OnDownloaded = delegate { };
        public bool Initialized { get; private set; }
        public void OnDownload(Action act)
        {
            OnDownloaded += act;
        }
        public Language(GID gid)
        {
            this.gid = gid;
            url = GetUrl(gid);
            dict = new Dictionary<string, string>();
            StaticCoroutine.Run(Download());
        }
        public string this[string key]
        {
            get => dict.TryGetValue(key, out string value) ? value : key;
            set => dict[key] = value;
        }
        public static Language GetLanguage(KeyViewerLanguage lang)
        {
            switch (lang)
            {
                case KeyViewerLanguage.English:
                    return English ??= new Language(GID.ENGLISH);
                case KeyViewerLanguage.Korean:
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
            JsonNode data = JsonNode.Parse(strData);
            JsonNode rows = data["table"]["rows"];
            Main.Logger.Log(rows.ToString(4));
            foreach (JsonNode row in rows)
            {
                JsonNode keyValue = row["c"];
                string key = keyValue[0]["v"];
                string value = keyValue[1]["v"];
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                    continue;
                dict.Add(key, value);
            }
            Main.Logger.Log($"Loaded {dict.Count} Localizations from Sheet");
            OnDownloaded();
            Initialized = true;
        }
        static string Escape(string str) => str.Replace(@"\", @"\\").Replace(":", @"\:");
    }
}
