using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Unity;

namespace KeyViewer.Views
{
    public class KeyConfigDrawer : ModelDrawable<KeyConfig>
    {
        public KeyManager manager;
        public KeyConfig config;
        public KeyConfigDrawer(KeyManager manager, KeyConfig config)
        {
            this.manager = manager;
            this.config = config;
        }
        public override void Draw(IDrawer drawer)
        {
            
        }
    }
}
