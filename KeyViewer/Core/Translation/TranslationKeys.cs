using UnityEngine;

namespace KeyViewer.Core.Translation
{
    public static class TranslationKeys
    {
        public static string Lorem_Ipsum => "LOREM_IPSUM";
        public static class Settings
        {
            public const string Prefix = "SETTINGS_";
            internal static readonly string SelectLanguage = Prefix + "SELECT_LANGUAGE";
            internal static readonly string Language = Prefix + "LANGUAGE";
            internal static readonly string ConfigurateProfile = Prefix + "CONFIGURATE_PROFILE";
            internal static readonly string CreateProfile = Prefix + "CREATE_PROFILE";
            internal static readonly string ImportProfile = Prefix + "IMPORT_PROFILE";
            internal static readonly string SelectProfile = Prefix + "SELECT_PROFILE";
        }
        public static class Profile
        {
            public const string Prefix = "PROFILE_";
            internal static readonly string MakeBarSpecialKeys = Prefix + "MAKE_BAR_SPECIAL_KEYS";
            internal static readonly string ViewOnlyGamePlay = Prefix + "VIEW_ONLY_GAME_PLAY";
            internal static readonly string AnimateKeys = Prefix + "ANIMATE_KEYS";
            internal static readonly string ShowKeyPressTotal = Prefix + "SHOW_KEY_PRESS_TOTAL";
            internal static readonly string LimitNotRegisteredKeys = Prefix + "LIMIT_NOT_REGISTERED_KEYS";
            internal static readonly string ResetOnStart = Prefix + "RESET_ON_START";
            internal static readonly string KPSUpdateRate = Prefix + "KPS_UPDATE_RATE";
            internal static readonly string Size = Prefix + "SIZE";
            internal static readonly string StartKeyRegistering = Prefix + "START_KEY_REGISTERING";
            internal static readonly string StopKeyRegistering = Prefix + "STOP_KEY_REGISTERING";
            internal static readonly string RegisterKPSKey = Prefix + "REGISTER_KPS_KEY";
            internal static readonly string RegisterTotalKey = Prefix + "REGISTER_TOTAL_KEY";
            internal static readonly string RegisterMouse0Key = Prefix + "REGISTER_MOUSE0_KEY";
        }
        public static class Misc
        {
            public const string Prefix = "MISC_";
        }
    }
}
