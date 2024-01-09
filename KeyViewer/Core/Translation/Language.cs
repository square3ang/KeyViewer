using KeyViewer.Models;
using KeyViewer.Utils;
using System;
using System.Linq;

namespace KeyViewer.Core.Translation
{
    public class Language
    {
        private const string KEY = "1wG6wB3q0q1E647mhECPSl5Sd_joqlsYOmkoPyDMs-Rw";
        private static Language Korean;
        private static Language English;
        private static SpreadSheet sheet = new SpreadSheet(KEY);
        public static event Action OnInitialize = delegate { };
        public static bool HasUpdate = false;
        public bool Initialized { get; private set; }
        public int Gid { get; }
        public Language(GID gid)
        {
            Gid = (int)gid;
            sheet.Download((int)gid, d =>
            {
                Version newVersion;
                if ((newVersion = Version.Parse(d[TranslationKeys.Version])) > Main.Mod.Version)
                {
                    HasUpdate = true;
                    var update = d[TranslationKeys.Update];
                    foreach (var key in d.Keys.Where(key =>
                    key != TranslationKeys.Lorem_Ipsum &&
                    key != TranslationKeys.Version &&
                    key != TranslationKeys.Update &&
                    key != TranslationKeys.DiscordLink &&
                    key != TranslationKeys.DownloadLink).ToList())
                        d[key] = update;
                    ErrorCanvasContext ecc = new ErrorCanvasContext();
                    ecc.titleText = "WOW YOUR KEYVIEWER VERSION IS BEAUTIFUL!";
                    ecc.errorMessage =
                        $"Current KeyViewer Version v{Main.Mod.Version}.\n" +
                        $"But Latest KeyViewer Is v{newVersion}.\n" +
                        $"PlEaSe UpDaTe YoUr KeYvIeWeR!";
                    ecc.ignoreBtnCallback = () =>
                    {
                        ADOUtils.HideError(ecc);
                        KeyViewerUtils.OpenDownloadUrl();
                    };
                    ADOUtils.ShowError(ecc);
                }
                Main.Logger.Log($"Loaded {d.Count} Localizations from Sheet");
                OnInitialize();
                Initialized = true;
            }).Await();
        }
        public string this[string key]
        {
            get => sheet[Gid, key];
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
