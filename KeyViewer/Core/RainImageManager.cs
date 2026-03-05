using KeyViewer.Models;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace KeyViewer.Core;

public class RainImageManager {
    public RainConfig config;
    private int count;
    private List<Sprite> sprites;
    public RainImageManager(RainConfig config) {
        this.config = config;
        sprites = [];
        Refresh();
    }
    public Sprite Get(out RainImage image) {
        var index = Index;
        image = count <= 0 ? null : config.RainImages[index];
        return count <= 0 ? null : sprites[index];
    }
    public void Refresh() {
        Index = count = 0;
        sprites = [];
        if(config.RainImages.Count > 0) {
            foreach(RainImage image in config.RainImages) {
                for(int i = 0; i < image.Count; i++) {
                    sprites.Add(AssetManager.Get(image.Image));
                }
            }

            count = sprites.Count;
            if(config.ImageDisplayMode == RainImageDisplayMode.Random) {
                int[] indexes = new int[count];
                for(int i = 0; i < count; indexes[i++] = (int)(count * URandom.value)) {
                    ;
                }

                for(int i = 0; i < count; i++) {
                    int target = indexes[i];

                    (sprites[target], sprites[i]) = (sprites[i], sprites[target]);
                }
            }
        }
    }
    int Index {
        get {
            if(field < sprites.Count) {
                return field++;
            }

            field = 1;
            return 0;
        }

        set;
    }
}
