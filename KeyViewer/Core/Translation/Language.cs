using JSON;
using KeyViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Core.Translation
{
    public class Language
    {
        private static Language Korean;
        private static Language English;
        private static Language Chinese;
        private static Language Japanese;
        public bool Initialized = false;
        public KeyViewerLanguage Lang;
        private bool updateMode = false;
        private Dictionary<string, string> pairs = new Dictionary<string, string>();
        public static event Action OnInitialize = delegate { };
        Language(KeyViewerLanguage lang)
        {
            Lang = lang;
            Download();
        }
        async void Download()
        {
            if (Initialized) return;
            var json = await KeyViewerWebAPI.GetLanguageJson(Lang);
            JsonNode node = JsonNode.Parse(json);
            foreach (var pair in node.KeyValues)
                pairs.Add(pair.Key, pair.Value);
            OnInitialize();
            Initialized = true;
            if (updateMode) ActivateUpdateMode();
        }
        public string this[string key]
        {
            get => pairs.TryGetValue(key, out var value) ? value : key;
            set => pairs[key] = value;
        }
        public void ActivateUpdateMode()
        {
            updateMode = true;
            if (Initialized)
            {
                foreach (var key in pairs.Keys.ToList())
                    pairs[key] = "Update!Update!Update!";
            }
        }
        public static Language GetLanguage(KeyViewerLanguage lang)
        {
            switch (lang)
            {
                case KeyViewerLanguage.Korean:
                    return Korean ??= new Language(KeyViewerLanguage.Korean);
                case KeyViewerLanguage.English:
                    return English ??= new Language(KeyViewerLanguage.English);
                case KeyViewerLanguage.Chinese:
                    return Chinese ??= new Language(KeyViewerLanguage.Chinese);
                case KeyViewerLanguage.Japanese:
                    return Japanese ??= new Language(KeyViewerLanguage.Japanese);
                default: return null;
            }
        }
    }
}
