namespace KeyViewer.Core.Translation
{
    public static class TranslationKeys
    {
        public static string Lorem_Ipsum => "LOREM_IPSUM";
        public static string Version = "V_E_R_S_I_O_N";
        public static string Update = "U_P_D_A_T_E";
        public static class Settings
        {
            public const string Prefix = "SETTINGS_";
            internal static readonly string SelectLanguage = Prefix + "SELECT_LANGUAGE";
            internal static readonly string Language = Prefix + "LANGUAGE";
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
            internal static readonly string StartKeyRegistering = Prefix + "START_KEY_REGISTERING";
            internal static readonly string StopKeyRegistering = Prefix + "STOP_KEY_REGISTERING";
            internal static readonly string RegisterKPSKey = Prefix + "REGISTER_KPS_KEY";
            internal static readonly string RegisterTotalKey = Prefix + "REGISTER_TOTAL_KEY";
            internal static readonly string RegisterMouse0Key = Prefix + "REGISTER_MOUSE0_KEY";
            internal static readonly string RegisteredKeys = Prefix + "REGISTERED_KEYS";
            internal static readonly string ConfigurationMode = Prefix + "CONFIGURATION_MODE";
            internal static readonly string ConfigurateProfile = Prefix + "CONFIGURATE_PROFILE";
            internal static readonly string CreateDummyKey = Prefix + "CREATE_DUMMY_KEY";
            internal static readonly string DummyName = Prefix + "DUMMY_NAME";
        }
        public static class Misc
        {
            public const string Prefix = "MISC_";
            internal static readonly string TopLeft = Prefix + "TOP_LEFT";
            internal static readonly string TopRight = Prefix + "TOP_RIGHT";
            internal static readonly string BottomLeft = Prefix + "BOTTOM_LEFT";
            internal static readonly string BottomRight = Prefix + "BOTTOM_RIGHT";
            internal static readonly string Pressed = Prefix + "PRESSED";
            internal static readonly string Released = Prefix + "RELEASED";
            internal static readonly string Enable = Prefix + "ENABLE";
            internal static readonly string Disable = Prefix + "DISABLE";
            internal static readonly string Scale = Prefix + "SCALE";
            internal static readonly string Offset = Prefix + "OFFSET";
            internal static readonly string Rotation = Prefix + "ROTATION";
            internal static readonly string ChangeColorWithJudge = Prefix + "CHANGE_COLOR_WITH_JUDGE";
            internal static readonly string ObjectConfigFrom = Prefix + "OBJECTCONFIG_FROM";
            internal static readonly string Edit = Prefix + "EDIT";
            internal static readonly string EditThis = Prefix + "EDIT_THIS";
            internal static readonly string JudgeColorFrom = Prefix + "JUDGECOLOR_FROM";
            internal static readonly string JudgeColor = Prefix + "JUDGECOLOR";
            internal static readonly string ThisColor = Prefix + "THIS_COLOR";
            internal static readonly string EnableGradient = Prefix + "ENABLE_GRADIENT";
            internal static readonly string TooEarly = Prefix + "TOO_EARLY";
            internal static readonly string VeryEarly = Prefix + "VERY_EARLY";
            internal static readonly string EarlyPerfect = Prefix + "EARLY_PERFECT";
            internal static readonly string Perfect = Prefix + "PERFECT";
            internal static readonly string LatePerfect = Prefix + "LATE_PERFECT";
            internal static readonly string VeryLate = Prefix + "VERY_LATE";
            internal static readonly string TooLate = Prefix + "TOO_LATE";
            internal static readonly string Multipress = Prefix + "MULTI_PRESS";
            internal static readonly string FailMiss = Prefix + "FAIL_MISS";
            internal static readonly string FailOverload = Prefix + "FAIL_OVERLOAD";
            internal static readonly string Size = Prefix + "SIZE";
            internal static readonly string Speed = Prefix + "SPEED";
            internal static readonly string Length = Prefix + "LENGTH";
            internal static readonly string Softness = Prefix + "SOFTNESS";
            internal static readonly string PoolSize = Prefix + "POOL_SIZE";
            internal static readonly string Ease = Prefix + "EASE";
            internal static readonly string Duration = Prefix + "DURATION";
            internal static readonly string EditEaseConfig = Prefix + "EDIT_EASE_CONFIG";
            internal static readonly string SizeEase = Prefix + "SIZE_EASE";
            internal static readonly string ScaleEase = Prefix + "SCALE_EASE";
            internal static readonly string OffsetEase = Prefix + "OFFSET_EASE";
            internal static readonly string RotationEase = Prefix + "ROTATION_EASE";
            internal static readonly string Color = Prefix + "COLOR";
            internal static readonly string CopyFromPressed = Prefix + "COPY_FROM_PRESSED";
            internal static readonly string CopyFromReleased = Prefix + "COPY_FROM_RELEASED";
            internal static readonly string Copy = Prefix + "COPY";
        }
        public static class KeyConfig
        {
            public const string Prefix = "KEYCONFIG_";
            internal static readonly string KeyConfiguration = Prefix + "KEY_CONFIGURATION";
            internal static readonly string DummyKeyName = Prefix + "DUMMY_KEY_NAME";
            internal static readonly string KeyCode = Prefix + "KEY_CODE";
            internal static readonly string TextFont = Prefix + "TEXT_FONT";
            internal static readonly string EnableKPSMeter = Prefix + "ENABLE_KPS_METER";
            internal static readonly string EnableCountText = Prefix + "ENABLE_COUNT_TEXT";
            internal static readonly string Text = Prefix + "TEXT";
            internal static readonly string CountText = Prefix + "COUNT_TEXT";
            internal static readonly string BackgroundImage = Prefix + "BACKGROUND_IMAGE";
            internal static readonly string OutlineImage = Prefix + "OUTLINE_IMAGE";
            internal static readonly string BackgroundImageRoundness = Prefix + "BACKGROUND_IMAGE_ROUNDNESS";
            internal static readonly string OutlineImageRoundness = Prefix + "OUTLINE_IMAGE_ROUNDNESS";
            internal static readonly string EditTextConfig = Prefix + "EDIT_TEXT_CONFIG";
            internal static readonly string Key = Prefix + "KEY";
            internal static readonly string KeyText = Prefix + "KEY_TEXT";
            internal static readonly string EditCountTextConfig = Prefix + "EDIT_COUNT_TEXT_CONFIG";
            internal static readonly string KeyCountText = Prefix + "KEY_COUNT_TEXT";
            internal static readonly string EditBackgroundConfig = Prefix + "EDIT_BACKGROUND_CONFIG";
            internal static readonly string KeyBackground = Prefix + "KEY_BACKGROUND";
            internal static readonly string EditOutlineConfig = Prefix + "EDIT_OUTLINE_CONFIG";
            internal static readonly string KeyOutline = Prefix + "KEY_OUTLINE";
            internal static readonly string EditRainConfig = Prefix + "EDIT_RAIN_CONFIG";
            internal static readonly string EnableRain = Prefix + "ENABLE_RAIN";
        }
        public static class RainConfig
        {
            public const string Prefix = "RAINCONFIG_";
            internal static readonly string KeyConfiguration = Prefix + "RAIN_CONFIGURATION";
            internal static readonly string RainSpeed = Prefix + "RAIN_SPEED";
            internal static readonly string RainLength = Prefix + "RAIN_LENGTH";
            internal static readonly string RainSoftness = Prefix + "RAIN_SOFTNESS";
            internal static readonly string RainPoolSize = Prefix + "RAIN_POOL_SIZE";
            internal static readonly string EditRainConfig = Prefix + "EDIT_RAIN_CONFIG";
            internal static readonly string KeyRain = Prefix + "KEY_RAIN";
            internal static readonly string ImageDisplayMode = Prefix + "IMAGE_DISPLAY_MODE";
            internal static readonly string Direction = Prefix + "DIRECTION";
            internal static readonly string RainImages = Prefix + "RAIN_IMAGES";
            internal static readonly string RainImagePath = Prefix + "RAIN_IMAGE_PATH";
            internal static readonly string RainImageCount = Prefix + "RAIN_IMAGE_COUNT";
        }
    }
}
