using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Models;
using KeyViewer.Unity;

namespace KeyViewer.Views
{
    public class KeyConfigDrawer : ModelDrawable<KeyConfig>
    {
        public KeyManager manager;
        public KeyConfigDrawer(KeyManager manager, KeyConfig config) : base(config)
        {
            this.manager = manager;
        }
        public override void Draw()
        {
            
        }
    }
}
