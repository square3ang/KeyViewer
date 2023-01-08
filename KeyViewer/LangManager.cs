using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyJson;
using UnityEngine;

namespace KeyViewer
{
    public class LangManager
    {
        public Languages langs;
        public LangManager(string langJson)
        {
            langs = langJson.FromJson<Languages>();
            langDict = new Dictionary<string, Language[]>();
            langDict["Global"] = langs.Global;
            langDict["KeyViewer"] = langs.KeyViewer;
            globalDict = new Dictionary<string, string>();
            globalDict2 = langDict["Global"].ToDictionary(l => l.Key);
            engDict = langDict["KeyViewer"].ToDictionary(l => l.Key, l => l.English);
            curDict = new Dictionary<string, string>();
        }
        public string GetGlobal(string key)
            => globalDict[key];
        public string GetLanguageName(LanguageType lang)
        {
            switch (lang)
            {
                case LanguageType.English:
                    return globalDict2["LANGUAGE_NAME"].English;
                case LanguageType.Korean:
                    return globalDict2["LANGUAGE_NAME"].Korean;
                case LanguageType.Spanish:
                    return globalDict2["LANGUAGE_NAME"].Spanish;
                case LanguageType.Polish:
                    return globalDict2["LANGUAGE_NAME"].Polish;
                case LanguageType.French:
                    return globalDict2["LANGUAGE_NAME"].French;
                case LanguageType.Vietnamese:
                    return globalDict2["LANGUAGE_NAME"].Vietnamese;
                case LanguageType.SimplifiedChinese:
                    return globalDict2["LANGUAGE_NAME"].SimplifiedChinese;
                default: throw new InvalidOperationException("Invalid Language!");
            }
        }
        public void ChangeLanguage(LanguageType lang)
        {
            switch (lang)
            {
                case LanguageType.English:
                    globalDict = langDict["Global"].ToDictionary(l => l.Key, l => l.English);
                    curDict = engDict;
                    return;
                case LanguageType.Korean:
                    globalDict = langDict["Global"].ToDictionary(l => l.Key, l => l.Korean);
                    curDict = langDict["KeyViewer"].ToDictionary(l => l.Key, l => l.Korean);
                    return;
                case LanguageType.Spanish:
                    globalDict = langDict["Global"].ToDictionary(l => l.Key, l => l.Spanish);
                    curDict = langDict["KeyViewer"].ToDictionary(l => l.Key, l => l.Spanish);
                    return;
                case LanguageType.Polish:
                    globalDict = langDict["Global"].ToDictionary(l => l.Key, l => l.Polish);
                    curDict = langDict["KeyViewer"].ToDictionary(l => l.Key, l => l.Polish);
                    return;
                case LanguageType.French:
                    globalDict = langDict["Global"].ToDictionary(l => l.Key, l => l.French);
                    curDict = langDict["KeyViewer"].ToDictionary(l => l.Key, l => l.French);
                    return;
                case LanguageType.Vietnamese:
                    globalDict = langDict["Global"].ToDictionary(l => l.Key, l => l.Vietnamese);
                    curDict = langDict["KeyViewer"].ToDictionary(l => l.Key, l => l.Vietnamese);
                    return;
                case LanguageType.SimplifiedChinese:
                    globalDict = langDict["Global"].ToDictionary(l => l.Key, l => l.SimplifiedChinese);
                    curDict = langDict["KeyViewer"].ToDictionary(l => l.Key, l => l.SimplifiedChinese);
                    return;
                default: throw new InvalidOperationException("Invalid Language!");
            }
        }
        public string GetString(string key)
        {
            var curValue = curDict[key];
            if (curValue == "ul")
                return engDict[key];
            return curValue;
        }
        private readonly Dictionary<string, Language[]> langDict;
        private Dictionary<string, string> globalDict;
        private Dictionary<string, Language> globalDict2;
        private Dictionary<string, string> curDict;
        private readonly Dictionary<string, string> engDict;
    }
}
