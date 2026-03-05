using KeyViewer.Core.Interfaces;
using KeyViewer.Utils;
using Newtonsoft.Json.Linq;

namespace KeyViewer.Models;

public class Judge<T> : IModel, ICopyable<Judge<T>> {
    public T TooEarly;
    public T VeryEarly;
    public T EarlyPerfect;
    public T Perfect;
    public T LatePerfect;
    public T VeryLate;
    public T TooLate;
    public T Multipress;
    public T FailMiss;
    public T FailOverload;
    public T Get(HitMargin hitMargin) {
        return hitMargin switch {
            HitMargin.TooEarly => TooEarly,
            HitMargin.VeryEarly => VeryEarly,
            HitMargin.EarlyPerfect => EarlyPerfect,
            HitMargin.Perfect => Perfect,
            HitMargin.LatePerfect => LatePerfect,
            HitMargin.VeryLate => VeryLate,
            HitMargin.TooLate => TooLate,
            HitMargin.Multipress => Multipress,
            HitMargin.FailMiss => FailMiss,
            HitMargin.FailOverload => FailOverload,
            _ => Perfect,
        };
    }
    public void Set(HitMargin hitMargin, T value) {
        switch(hitMargin) {
            case HitMargin.TooEarly:
                TooEarly = value;
                break;
            case HitMargin.VeryEarly:
                VeryEarly = value;
                break;
            case HitMargin.EarlyPerfect:
                EarlyPerfect = value;
                break;
            case HitMargin.Perfect:
                Perfect = value;
                break;
            case HitMargin.LatePerfect:
                LatePerfect = value;
                break;
            case HitMargin.VeryLate:
                VeryLate = value;
                break;
            case HitMargin.TooLate:
                TooLate = value;
                break;
            case HitMargin.Multipress:
                Multipress = value;
                break;
            case HitMargin.FailMiss:
                FailMiss = value;
                break;
            case HitMargin.FailOverload:
                FailOverload = value;
                break;
            default:
                Perfect = value;
                break;
        }
    }
    public JToken Serialize() {
        var node = new JObject {
            [nameof(TooEarly)] = ModelUtils.ToNode<T>(TooEarly),
            [nameof(VeryEarly)] = ModelUtils.ToNode<T>(VeryEarly),
            [nameof(EarlyPerfect)] = ModelUtils.ToNode<T>(EarlyPerfect),
            [nameof(Perfect)] = ModelUtils.ToNode<T>(Perfect),
            [nameof(LatePerfect)] = ModelUtils.ToNode<T>(LatePerfect),
            [nameof(VeryLate)] = ModelUtils.ToNode<T>(VeryLate),
            [nameof(TooLate)] = ModelUtils.ToNode<T>(TooLate),
            [nameof(Multipress)] = ModelUtils.ToNode<T>(Multipress),
            [nameof(FailMiss)] = ModelUtils.ToNode<T>(FailMiss),
            [nameof(FailOverload)] = ModelUtils.ToNode<T>(FailOverload)
        };
        return node;
    }
    public void Deserialize(JToken node) {
        TooEarly = (T)ModelUtils.ToObject<T>(node[nameof(TooEarly)]);
        VeryEarly = (T)ModelUtils.ToObject<T>(node[nameof(VeryEarly)]);
        EarlyPerfect = (T)ModelUtils.ToObject<T>(node[nameof(EarlyPerfect)]);
        Perfect = (T)ModelUtils.ToObject<T>(node[nameof(Perfect)]);
        LatePerfect = (T)ModelUtils.ToObject<T>(node[nameof(LatePerfect)]);
        VeryLate = (T)ModelUtils.ToObject<T>(node[nameof(VeryLate)]);
        TooLate = (T)ModelUtils.ToObject<T>(node[nameof(TooLate)]);
        Multipress = (T)ModelUtils.ToObject<T>(node[nameof(Multipress)]);
        FailMiss = (T)ModelUtils.ToObject<T>(node[nameof(FailMiss)]);
        FailOverload = (T)ModelUtils.ToObject<T>(node[nameof(FailOverload)]);
    }
    public Judge<T> Copy() {
        var newJudge = new Judge<T> {
            TooEarly = TooEarly,
            VeryEarly = VeryEarly,
            EarlyPerfect = EarlyPerfect,
            Perfect = Perfect,
            LatePerfect = LatePerfect,
            VeryLate = VeryLate,
            TooLate = TooLate,
            Multipress = Multipress,
            FailMiss = FailMiss,
            FailOverload = FailOverload
        };
        return newJudge;
    }
}
public class JudgeM<T> : Judge<T>, IModel, ICopyable<JudgeM<T>> where T : IModel, ICopyable<T>, new() {
    public new JObject Serialize() {
        var node = new JObject {
            [nameof(TooEarly)] = TooEarly.Serialize(),
            [nameof(VeryEarly)] = VeryEarly.Serialize(),
            [nameof(EarlyPerfect)] = EarlyPerfect.Serialize(),
            [nameof(Perfect)] = Perfect.Serialize(),
            [nameof(LatePerfect)] = LatePerfect.Serialize(),
            [nameof(VeryLate)] = VeryLate.Serialize(),
            [nameof(TooLate)] = TooLate.Serialize(),
            [nameof(Multipress)] = Multipress.Serialize(),
            [nameof(FailMiss)] = FailMiss.Serialize(),
            [nameof(FailOverload)] = FailOverload.Serialize()
        };
        return node;
    }
    public new void Deserialize(JToken node) {
        TooEarly = ModelUtils.Unbox<T>(node[nameof(TooEarly)]);
        VeryEarly = ModelUtils.Unbox<T>(node[nameof(VeryEarly)]);
        EarlyPerfect = ModelUtils.Unbox<T>(node[nameof(EarlyPerfect)]);
        Perfect = ModelUtils.Unbox<T>(node[nameof(Perfect)]);
        LatePerfect = ModelUtils.Unbox<T>(node[nameof(LatePerfect)]);
        VeryLate = ModelUtils.Unbox<T>(node[nameof(VeryLate)]);
        TooLate = ModelUtils.Unbox<T>(node[nameof(TooLate)]);
        Multipress = ModelUtils.Unbox<T>(node[nameof(Multipress)]);
        FailMiss = ModelUtils.Unbox<T>(node[nameof(FailMiss)]);
        FailOverload = ModelUtils.Unbox<T>(node[nameof(FailOverload)]);
    }
    public new JudgeM<T> Copy() {
        var newJudge = new JudgeM<T> {
            TooEarly = TooEarly.Copy(),
            VeryEarly = VeryEarly.Copy(),
            EarlyPerfect = EarlyPerfect.Copy(),
            Perfect = Perfect.Copy(),
            LatePerfect = LatePerfect.Copy(),
            VeryLate = VeryLate.Copy(),
            TooLate = TooLate.Copy(),
            Multipress = Multipress.Copy(),
            FailMiss = FailMiss.Copy(),
            FailOverload = FailOverload.Copy()
        };
        return newJudge;
    }
}