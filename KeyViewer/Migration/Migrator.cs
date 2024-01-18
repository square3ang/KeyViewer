using KeyViewer.Migration.V2;
using KeyViewer.Migration.V3;

namespace KeyViewer.Migration
{
    public sealed class Migrator
    {
        public static V2Migrator V2(string keyCountsPath, string keySettingsPath, string settingsPath) => new V2Migrator(keyCountsPath, keySettingsPath, settingsPath);
    }
}
