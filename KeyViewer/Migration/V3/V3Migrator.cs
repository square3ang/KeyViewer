using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KeyViewer.Migration.V3
{
    // V3 To V4
    public class V3Migrator
    {
        public V3Settings Settings { get; private set; }
        public V3Migrator(string settingsPath)
        {
            if (!string.IsNullOrWhiteSpace(settingsPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(V3Settings));
                Settings = (V3Settings)serializer.Deserialize(File.Open(settingsPath, FileMode.Open));
            }
            else throw new InvalidOperationException("Settings Path Cannot Be Null!");
        }

    }
}
