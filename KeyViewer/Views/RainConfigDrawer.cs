using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Core;
using KeyViewer.Core.Interfaces;
using KeyViewer.Core.Translation;

namespace KeyViewer.Views
{
    public class RainConfigDrawer : ModelDrawable
    {
        public RainConfig config;
        public RainConfigDrawer(KeyManager manager, RainConfig config) : base(manager)
        {
            this.config = config;
        }
        public override void Draw(IDrawer drawer)
        {
            var offset = config.Offset;
            ExpandableGUI(() =>
            {

            }, () =>
            {

            }, Main.Lang[TranslationKeys.SeparatePressedReleased], ref offset.Enabled, ref offset.Expanded);
        }
    }
}
