using Verse;

namespace Amnabi;

public class PR_TF_FalangeCross : PR_ToolFormation
{
    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        var bestColor = fft.getBestColor(layerNow, "LongStick");
        var fpd = HarmonyFlags.PatternByTags["LongStick"]
            .RandomElementByWeight(x => IntersectionAdjustedPoints(x, fft));
        var fpd2 = HarmonyFlags.PatternByTags["LongStick"]
            .RandomElementByWeight(x => IntersectionAdjustedPoints(x, fft));
        var fpd3 = HarmonyFlags.PatternByTags["LongStick"]
            .RandomElementByWeight(x => IntersectionAdjustedPoints(x, fft));
        var fpd4 = HarmonyFlags.PatternByTags["LongStick"]
            .RandomElementByWeight(x => IntersectionAdjustedPoints(x, fft));
        var fpd5 = HarmonyFlags.PatternByTags["LongStick"]
            .RandomElementByWeight(x => IntersectionAdjustedPoints(x, fft));
        layerNow.attachPost(layerNow.createFlagPatternMinScaled(fpd, bestColor, 0.5, 0.5, 45.0));
        layerNow.attachPost(layerNow.createFlagPatternMinScaled(fpd4, bestColor, 0.5, 0.5, 22.5));
        layerNow.attachPost(layerNow.createFlagPatternMinScaled(fpd2, bestColor, 0.5, 0.5, 0.0));
        layerNow.attachPost(layerNow.createFlagPatternMinScaled(fpd5, bestColor, 0.5, 0.5, -22.5));
        layerNow.attachPost(layerNow.createFlagPatternMinScaled(fpd3, bestColor, 0.5, 0.5, -45.0));
        layerNow.setBackgroundColorUpdate(bestColor);
    }
}