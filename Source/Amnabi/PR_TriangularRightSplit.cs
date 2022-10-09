using UnityEngine;

namespace Amnabi;

public class PR_TriangularRightSplit : PR_Split
{
    public override double getProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        if (layerNow.innerrect.width > layerNow.innerrect.height)
        {
            return base.getProbability(fft, layerNow, depth) * Mathf.Min(layerNow.hRatio(), 1f);
        }

        return base.getProbability(fft, layerNow, depth) * Mathf.Min(layerNow.wRatio(), 1f);
    }

    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        var choosen = fft.getBestColor(layerNow, "Background");
        layerNow.attachPre(layerNow.createFlagPattern(AmnabiFlagDefOfs.Pattern_TriangleA, choosen,
            0f - layerNow.innerrect.width, layerNow.innerrect.height, 0.5, 1.0, 0.0));
        ExecRandOrder(delegate
        {
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x + (layerNow.innerrect.width / 2f), layerNow.innerrect.y,
                        layerNow.innerrect.width / 2f, layerNow.innerrect.height / 2f), depth).tagInc("SplitSub", 1)
                .tagInc("SplitSubVert", 1)
                .iterateLayer(fft, depth);
        }, delegate
        {
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x, layerNow.innerrect.y + (layerNow.innerrect.height / 2f),
                        layerNow.innerrect.width / 2f, layerNow.innerrect.height / 2f), depth).tagInc("SplitSub", 1)
                .tagInc("SplitSubVert", 1)
                .setBackgroundColorUpdate(choosen)
                .iterateLayer(fft, depth);
        });
    }
}