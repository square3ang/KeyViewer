using KeyViewer.Models;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace KeyViewer.Core
{
    public class RainImageManager
    {
        public RainConfig config;
        private int index;
        private int count;
        private List<Sprite> sprites;
        private List<float> roundness;
        public RainImageManager(RainConfig config)
        {
            this.config = config;
            sprites = new List<Sprite>();
            roundness = new List<float>();
            Refresh();
        }
        public Sprite Get() => count <= 0 ? null : sprites[Index];
        public float GetLastRoundness() => count <= 0 ? 0 : roundness[index];
        public void Refresh()
        {
            index = count = 0;
            sprites = new List<Sprite>();
            if (config.RainImages.Count > 0)
            {
                foreach (RainImage image in config.RainImages)
                    for (int i = 0; i < image.Count; i++)
                    {
                        sprites.Add(AssetManager.Get(image.Image));
                        roundness.Add(image.Roundness);
                    }
                count = sprites.Count;
                if (config.ImageDisplayMode == RainImageDisplayMode.Random)
                {
                    int[] indexes = new int[count];
                    for (int i = 0; i < count; indexes[i++] = (int)(count * URandom.value)) ;
                    for (int i = 0; i < count; i++)
                    {
                        int target = indexes[i];

                        Sprite temp = sprites[i];
                        sprites[i] = sprites[target];
                        sprites[target] = temp;

                        float tempR = roundness[i];
                        roundness[i] = roundness[target];
                        roundness[target] = tempR;
                    }
                }
            }
        }
        int Index
        {
            get
            {
                if (index < sprites.Count)
                    return index++;
                index = 1;
                return 0;
            }
        }
    }
}
