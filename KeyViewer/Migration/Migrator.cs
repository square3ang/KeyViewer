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
        public static V2Migrator V2(V2MigratorArgument v2Arg)
           => new V2Migrator(v2Arg);
    }
}
