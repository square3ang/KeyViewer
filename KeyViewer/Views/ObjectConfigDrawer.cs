using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using Overlayer.Core;

namespace KeyViewer.Views;

public class ObjectConfigDrawer(KeyManager manager, KeyConfig config, ObjectConfig objectConfing) :
    ModelDrawable<ObjectConfig>(objectConfing, string.Format(
        Main.Lang.Get("OBJECT_CONFIGURATION", "Key {0} {1} Configuration"),
        (config.DummyName != null ? config.DummyName : config.Code), GuessLabel(config, objectConfing))
    ) {

    public KeyManager manager = manager;

    public override void OnceCall() => NeoDrawer.StaticInstance.FieldResetDictById();

    public override void Draw() {
        NeoDrawer.StaticInstance.FieldResetId();

        bool changed = false;

        changed |= NeoDrawer.StaticInstance.DrawObjectConfig(model);

        if(changed) {
            manager.UpdateLayout();
        }

        NeoDrawer.StaticInstance.UpdateFocused();
    }

    static string GuessLabel(KeyConfig k, ObjectConfig obj) {
        if(obj == k.TextConfig) {
            return Main.Lang.Get("TEXT", "Text");
        }
        if(obj == k.CountTextConfig) {
            return Main.Lang.Get("COUNT_TEXT", "Count Text");
        }
        if(obj == k.BackgroundConfig) {
            return Main.Lang.Get("BACKGROUND_IMAGE", "Background Image");
        }
        if(obj == k.OutlineConfig) {
            return Main.Lang.Get("OUTLINE_IMAGE", "Outline Image");
        }

        return "Object";
    }
}
