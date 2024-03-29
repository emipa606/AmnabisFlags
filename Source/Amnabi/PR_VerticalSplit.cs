using UnityEngine;

namespace Amnabi;

public class PR_VerticalSplit : PR_Split
{
    protected override double getProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        return base.getProbability(fft, layerNow, depth) * Mathf.Min(layerNow.wRatio(), 1f);
    }
}