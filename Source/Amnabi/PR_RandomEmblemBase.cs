using Verse;

namespace Amnabi;

public class PR_RandomEmblemBase : PR_EmblemBase
{
    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        if (Rand.Value < 0.3f)
        {
            return;
        }

        var bestColor = fft.getBestColor(layerNow, "EmblemBase");
        var fpd = Harmony_Flags.patternByTags["EmblemBase"]
            .RandomElementByWeight(x => IntersectionAdjustedPoints(x, fft));
        layerNow.attachPost(layerNow.createFlagPatternMinScaled(fpd, bestColor, 0.5, 0.5, 0.0))
            .setBackgroundColorUpdate(bestColor);
    }
}