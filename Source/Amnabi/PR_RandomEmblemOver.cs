using Verse;

namespace Amnabi;

public class PR_RandomEmblemOver : PR_EmblemOver
{
    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        if (Rand.Value < 0.3f)
        {
            return;
        }

        var bestColor = fft.getBestColor(layerNow, "EmblemTop");
        var fpd = Harmony_Flags.patternByTags["EmblemTop"]
            .RandomElementByWeight(x => IntersectionAdjustedPoints(x, fft));
        var scale = layerNow.MinDims() * 0.8f;
        layerNow.attachPost(layerNow.createFlagPattern(fpd, bestColor, scale, 0.5, 0.5, 0.0))
            .setBackgroundColorUpdate(bestColor);
    }
}