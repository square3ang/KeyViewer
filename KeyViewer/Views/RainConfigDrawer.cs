using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using Overlayer.Core;

namespace KeyViewer.Views;

public class RainConfigDrawer : ModelDrawable<RainConfig> {

    public KeyManager manager;
    public KeyConfig config;
    public RainConfigDrawer(KeyManager manager, KeyConfig config) : base(config.Rain, string.Format(Main.Lang.Get("RAIN_CONFIGURATION", "Key {0} Rain Configuration"), config.DummyName != null ? config.DummyName : config.Code)) {
        this.manager = manager;
        this.config = config;
    }

    public override void OnceCall() => NeoDrawer.StaticInstance.FieldResetDictById();

    public override void Draw() {
        NeoDrawer.StaticInstance.FieldResetId();

        bool changed = false;

        changed |= NeoDrawer.StaticInstance.DrawRainConfig(model);

        if(changed) {
            manager.UpdateLayout();
        }

        NeoDrawer.StaticInstance.UpdateFocused();
    }
}
