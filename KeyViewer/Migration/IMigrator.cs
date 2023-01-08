using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyViewer.Migration
{
    public interface IMigrator
    {
        Settings Migrate();
    }
}
