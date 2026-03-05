using KeyViewer.Models;
using KeyViewer.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer.Unity;

public class Rain : MonoBehaviour {
    public bool IsAlive { get; private set; }
    public Vector2 Position;
    public Vector2 DefaultSize;

    internal RectTransform rt;
    //internal bool blurEnabled => config.BlurEnabled || (rImage?.BlurEnabled ?? false);

    private bool stretching = false;
    private RainConfig config;
    private Key key;
    private ObjectConfig objConfig;
    private RainImage rImage;
    private int colorUpdateIgnores;
    private bool initialized = false;

    internal Image image;
    public void Init(Key key) {
        if(initialized) {
            return;
        }

        this.key = key;
        image = gameObject.AddComponent<Image>();
        rt = image.rectTransform;
        config = key.Config.Rain;
        objConfig = config.ObjectConfig;

        OnEnable();
        KeyViewerUtils.ApplyColorLayout(image, objConfig.Color.Released, false);
        KeyViewerUtils.ApplyConfigLayout(this, objConfig.VectorConfig, DefaultSize, false);
        initialized = true;
    }
    public void Press() {
        if(!initialized || stretching) {
            return;
        }

        stretching = true;
        image.sprite = key.RainImageManager.Get(out rImage);
        var color = config.ObjectConfig.Color;
        if(colorUpdateIgnores == 0) {
            KeyViewerUtils.ApplyColor(image, color.Released, color.Pressed, color.PressedEase, false);
        } else {
            colorUpdateIgnores--;
        }
        //KeyViewerUtils.ApplyVectorConfig(rt, objConfig.VectorConfig, true, Position, false, DefaultSize, false);
        KeyViewerUtils.ApplyVectorConfig(rt, objConfig.VectorConfig, true, false, DefaultSize, false);
    }
    public void Release() {
        if(!initialized || !stretching) {
            return;
        }

        stretching = false;
        var color = config.ObjectConfig.Color;
        if(colorUpdateIgnores == 0) {
            KeyViewerUtils.ApplyColor(image, color.Pressed, color.Released, color.ReleasedEase, false);
        } else {
            colorUpdateIgnores--;
        }
        //Vector2 adjustedPosition = KeyViewerUtils.AdjustRainPosition(config.Direction, Position, objConfig.VectorConfig.Offset.Pressed);
        //KeyViewerUtils.ApplyVectorConfig(rt, objConfig.VectorConfig, false, adjustedPosition, false, DefaultSize, false);
        KeyViewerUtils.ApplyVectorConfig(rt, objConfig.VectorConfig, false, false, DefaultSize, false);
    }
    public void OnEnable() {
        if(!initialized) {
            return;
        }

        colorUpdateIgnores = 0;
        rt.sizeDelta = DefaultSize = GetInitialSize();
        rt.anchoredPosition = GetPosition(config.Direction);
        Position = rt.localPosition;
        //if (rImage != null)
        //{
        //    var lastBlur = rImage?.BlurConfig ?? config.BlurConfig;
        //    KeyViewerUtils.ApplyRoundnessBlurLayout(image, ref rImage.Roundness == 0 ? ref config.Roundness : ref rImage.Roundness, lastBlur, blurEnabled);
        //}
        //else KeyViewerUtils.ApplyRoundnessBlurLayout(image, ref config.Roundness, config.BlurConfig, config.BlurEnabled);
        KeyViewerUtils.ApplyRoundnessBlurLayout(image, ref config.Roundness, null, false);
    }
    public void IgnoreColorUpdate() => colorUpdateIgnores++;
    private void Update() {
        IsAlive = IsVisible(config.Direction);
        if(IsAlive) {
            var toMove = Time.deltaTime * config.Speed.Get(key.Pressed);
            var delta = GetDelta(config.Direction, toMove);
            if(stretching) {
                rt.sizeDelta += delta.Abs();
                rt.anchoredPosition += delta * 0.5f;
                DefaultSize = rt.sizeDelta;
            } else {
                rt.anchoredPosition += delta;
            }

            Position = rt.localPosition;
        } else {
            stretching = false;
            OnEnable();
            gameObject.SetActive(false);
        }
    }
    private bool IsVisible(Direction dir) {
        return dir switch {
            Direction.Up => rt.anchoredPosition.y - rt.sizeDelta.y <= config.Length.Get(key.Pressed),
            Direction.Down => -rt.anchoredPosition.y - rt.sizeDelta.y <= config.Length.Get(key.Pressed),
            Direction.Left => -rt.anchoredPosition.x - rt.sizeDelta.x <= config.Length.Get(key.Pressed),
            Direction.Right => rt.anchoredPosition.x - rt.sizeDelta.x <= config.Length.Get(key.Pressed),
            _ => false,
        };
    }
    private Vector2 GetInitialSize() {
        Vector2 scale = objConfig.VectorConfig.Scale.Get(key.Pressed);
        return config.Direction switch {
            Direction.Up or Direction.Down => scale.x > 0 ?
                                    new Vector2(key.Size.x * scale.x, 0) :
                                    new Vector2(key.Size.x, 0),
            Direction.Left or Direction.Right => scale.y > 0 ?
                                    new Vector2(0, key.Size.y * scale.y) :
                                    new Vector2(0, key.Size.y),
            _ => Vector2.zero,
        };
    }
    private Vector2 GetDelta(Direction dir, float value) {
        return dir switch {
            Direction.Up => new Vector2(0, value),
            Direction.Down => new Vector2(0, -value),
            Direction.Left => new Vector2(-value, 0),
            Direction.Right => new Vector2(value, 0),
            _ => Vector2.zero,
        };
    }
    private Vector2 GetPosition(Direction dir) {
        var sizeDelta = key.RainMaskRt.sizeDelta;
        return dir switch {
            Direction.Up => new Vector2(0, (-sizeDelta.y / 2) + config.Softness.Get(key.Pressed)),
            Direction.Down => new Vector2(0, (sizeDelta.y / 2) - config.Softness.Get(key.Pressed)),
            Direction.Left => new Vector2((sizeDelta.x / 2) - config.Softness.Get(key.Pressed), 0),
            Direction.Right => new Vector2((-sizeDelta.x / 2) + config.Softness.Get(key.Pressed), 0),
            _ => Vector2.zero,
        };
    }
}
