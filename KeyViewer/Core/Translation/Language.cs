using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using System;
using System.Linq;

namespace KeyViewer.Core.Translation
{
    public class Language
    {
        private const string KEY = "1EiWVds23-gZeRCrXL-UYr-o-sc0m-jfqWa-G7qmUYdI";
        internal static SpreadSheet sheet = new SpreadSheet(KEY);
        private static Language Korean;
        private static Language English = new Language(GID.ENGLISH);
        private static Language Chinese;
        private static Language Japanese;
        public static event Action OnInitialize = delegate { };
        public bool Initialized { get; private set; }
        public int Gid { get; }
        public Language(GID gid)
        {
            Gid = (int)gid;
            sheet.Download((int)gid, d =>
            {
                Main.Logger.Log($"Loaded {d.Count} Localizations from Sheet (GID:{(GID)Gid})");
                OnInitialize();
                Initialized = true;
            }).Await();
        }
        public string this[string key]
        {
            get => string.IsNullOrEmpty(sheet[Gid, key]) ? English[key] : sheet[Gid, key];
            set => sheet[Gid, key] = value;
        }
        public static Language GetLanguage(KeyViewerLanguage lang)
        {
            switch (lang)
            {
                case KeyViewerLanguage.English:
                    return English ??= new Language(GID.ENGLISH);
                case KeyViewerLanguage.Korean:
                    return Korean ??= new Language(GID.KOREAN);
                case KeyViewerLanguage.Chinese:
                    return Chinese ??= new Language(GID.CHINESE);
                case KeyViewerLanguage.Japanese:
                    return Japanese ??= new Language(GID.JAPANESE);
                default: return English;
            }
        }
        public static void Release()
        {
            Korean = null;
            English = null;
        }
    }
}
