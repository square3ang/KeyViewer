using KeyViewer.Core;
using KeyViewer.Models;
using KeyViewer.Unity;
using KeyViewer.Utils;
using Overlayer.Core;
using System.Collections.Generic;
using System.Linq;

namespace KeyViewer.Views;

public class MultipleRainConfigDrawer : ModelDrawable<RainConfig> {
    public KeyManager manager;
    public RainConfig modelCopy;
    public List<RainConfig> targets;
    public List<RainConfig> targetsCopy;

    public MultipleRainConfigDrawer(KeyManager manager, List<string> targetNames, RainConfig criterion)
        : base(criterion ?? new RainConfig(),
              string.Format(Main.Lang.Get("RAIN_CONFIGURATION", "Key {0} Rain Configuration"),
              KeyViewerUtils.AggregateComma(targetNames))) {
        this.manager = manager;

        modelCopy = model.Copy();

        targets = manager.keys
            .Where(k => targetNames.Contains(KeyViewerUtils.KeyName(k.Config)))
            .Select(k => k.Config.Rain)
            .ToList();

        targetsCopy = targets.Select(t => t.Copy()).ToList();
    }

    public override void Draw() {
        NeoDrawer.StaticInstance.FieldResetId();

        bool changed = false;

        changed |= NeoDrawer.StaticInstance.DrawRainConfig(model);

        if(changed) {
            foreach(var t in targets) {
                t.PoolSize = model.PoolSize;
                t.Roundness = model.Roundness;
                t.Speed = model.Speed.Copy();
                t.Length = model.Length.Copy();
                t.Softness = model.Softness.Copy();
                t.ObjectConfig = model.ObjectConfig.Copy();
                t.RainImages = model.RainImages.Select(r => r.Copy()).ToList();
                t.Direction = model.Direction;
                t.ImageDisplayMode = model.ImageDisplayMode;
            }

            manager.UpdateLayout();
        }

        NeoDrawer.StaticInstance.UpdateFocused();
    }
}