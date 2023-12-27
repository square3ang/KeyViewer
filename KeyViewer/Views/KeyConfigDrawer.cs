using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Unity;

namespace KeyViewer.Views
{
    public class KeyConfigDrawer : ModelDrawable<KeyConfig>
    {
        public KeyConfig config;
        public KeyConfigDrawer(KeyManager manager, KeyConfig config) : base(manager)
        {
            this.config = config;
        }
        public override void Draw(IDrawer drawer)
        {
            
        }
    }
}
