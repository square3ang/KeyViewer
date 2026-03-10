using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using Overlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Views;

public class MultipleObjectConfigDrawer : ModelDrawable<ObjectConfig> {
    public KeyManager manager;
    public ObjectConfig modelCopy;
    public List<ObjectConfig> targets;
    public List<ObjectConfig> targetsCopy;
    public Func<KeyConfig, ObjectConfig> selector;

    public MultipleObjectConfigDrawer(
    KeyManager manager,
    List<string> targetNames,
    ObjectConfig criterion,
    Func<KeyConfig, ObjectConfig> selector
    ) : base(
        criterion ?? new ObjectConfig(),
        string.Format(
            Main.Lang.Get("OBJECT_CONFIGURATION", "Key {0} {1} Configuration"),
            KeyViewerUtils.AggregateComma(targetNames),
            GuessLabel(manager, targetNames, selector)
        )
    ) {
        this.manager = manager;
        this.selector = selector;

        modelCopy = model.Copy();

        targets = manager.keys
            .Where(k => targetNames.Contains(KeyViewerUtils.KeyName(k.Config)))
            .Select(k => selector(k.Config))
            .ToList();

        targetsCopy = targets.Select(t => t.Copy()).ToList();
    }

    public override void OnceCall() {
        NeoDrawer.StaticInstance.FieldResetDictById();
    }

    public override void Draw() {
        NeoDrawer.StaticInstance.FieldResetId();

        bool changed = false;

        changed |= NeoDrawer.StaticInstance.DrawObjectConfig(model);

        if(changed) {
            foreach(var t in targets) {
                t.Color = model.Color;
                t.VectorConfig = model.VectorConfig.Copy();
            }

            manager.UpdateLayout();
        }

        NeoDrawer.StaticInstance.UpdateFocused();
    }

    static string GuessLabel(KeyManager manager, List<string> targetNames, Func<KeyConfig, ObjectConfig> selector) {
        var key = manager.keys.First(k => targetNames.Contains(KeyViewerUtils.KeyName(k.Config)));
        var cfg = key.Config;
        var obj = selector(cfg);

        if(obj == cfg.TextConfig) {
            return Main.Lang.Get("TEXT", "Text");
        }
        if(obj == cfg.CountTextConfig) {
            return Main.Lang.Get("COUNT_TEXT", "Count Text");
        }
        if(obj == cfg.BackgroundConfig) {
            return Main.Lang.Get("BACKGROUND_IMAGE", "Background Image");
        }
        if(obj == cfg.OutlineConfig) {
            return Main.Lang.Get("OUTLINE_IMAGE", "Outline Image");
        }

        return "Object";
    }
}