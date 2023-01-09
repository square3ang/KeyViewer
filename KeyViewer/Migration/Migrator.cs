using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyViewer.Migration.V2;

namespace KeyViewer.Migration
{
    public sealed class Migrator
    {
        public static IMigrator V2(KeyManager manager, string keyCountsPath, string keySettingsPath, string settingsPath)
            => new V2Migrator(manager, keyCountsPath, keySettingsPath, settingsPath);
        public static IMigrator V2(KeyManager manager, V2MigratorArgument v2Arg)
           => new V2Migrator(manager, v2Arg);
    }
}
