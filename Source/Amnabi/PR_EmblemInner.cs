namespace Amnabi;

public class PR_EmblemInner : PatternRecursive
{
    protected override double getPreProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        return layerNow.tag(PR_Emblem.EMBLEMINNERMUST) > 0 ? 1 : 0;
    }

    protected override double getProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        return 0.0;
    }

    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        if (layerNow.tag(PR_Emblem.EMBLEMINNERMUST) > 0)
        {
            layerNow.tagInc(PR_Emblem.EMBLEMINNERMUST, -1);
        }
    }
}