using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Core.Translation;
using TKRC = KeyViewer.Core.Translation.TranslationKeys.RainConfig;

namespace KeyViewer.Views
{
    public class RainConfigDrawer : ModelDrawable<RainConfig>
    {
        public KeyManager manager;
        public RainConfigDrawer(KeyManager manager, KeyConfig config) : base(config.Rain, L(TKRC.KeyConfiguration, config.DummyName != null ? config.DummyName : config.Code))
        {
            this.manager = manager;
        }
        public override void Draw()
        {

        }
    }
}
