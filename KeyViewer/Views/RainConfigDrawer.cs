using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Core.Translation;

namespace KeyViewer.Views
{
    public class RainConfigDrawer : ModelDrawable<RainConfig>
    {
        public KeyManager manager;
        public RainConfig config;
        public RainConfigDrawer(KeyManager manager, RainConfig config)
        {
            this.manager = manager;
            this.config = config;
        }
        public override void Draw(IDrawer drawer)
        {

        }
    }
}
