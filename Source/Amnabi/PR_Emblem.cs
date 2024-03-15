using UnityEngine;

namespace Amnabi;

public class PR_Emblem : PatternRecursive
{
    public static readonly string EMBLEMINNERMUST = "EmblemInnerMust";

    public static readonly string EMBLEMBACKMUST = "EmblemBackgroundMust";

    public static readonly string EMBLEMOVERMUST = "EmblemOverMust";

    public static readonly string EMBLEMDONE = "EmblemDone";

    public bool recurseLayer = false;

    protected override double getProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        var num = layerNow.tagT(EMBLEMDONE);
        var num2 = num switch
        {
            0 => 6f,
            1 => 0.01f,
            _ => 0f
        };
        return layerNow.tag(EMBLEMDONE) > 0 ? 0f : 2f * num2 * Mathf.Min(layerNow.minRatio() * 1.5f, 1f);
    }

    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        layerNow.tagInc(EMBLEMDONE, 1);
    }
}