using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using TKKC = KeyViewer.Core.Translation.TranslationKeys.KeyConfig;

namespace KeyViewer.Views
{
    public class KeyConfigDrawer : ModelDrawable<KeyConfig>
    {
        public KeyManager manager;
        public KeyConfigDrawer(KeyManager manager, KeyConfig config) : base(config, L(TKKC.KeyConfiguration, config.SpecialKey != SpecialKeyType.None ? config.SpecialKey : config.Code))
        {
            this.manager = manager;
        }
        public override void Draw()
        {
            
        }
    }
}
