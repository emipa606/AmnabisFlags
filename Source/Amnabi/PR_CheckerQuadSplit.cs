using UnityEngine;

namespace Amnabi;

public class PR_CheckerQuadSplit : PR_Split
{
    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        ExecRandOrder(
            delegate
            {
                layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x, layerNow.innerrect.y, layerNow.innerrect.width / 2f,
                        layerNow.innerrect.height / 2f), depth).tagInc("SplitSub", 1).iterateLayer(fft, depth);
            }, delegate
            {
                layerNow.childLayer(this,
                        new Rect(layerNow.innerrect.x + (layerNow.innerrect.width / 2f), layerNow.innerrect.y,
                            layerNow.innerrect.width / 2f, layerNow.innerrect.height / 2f), depth)
                    .tagInc("SplitSub", 1).setBackground(fft.getRandomColor("Background", MI(1, depth)))
                    .iterateLayer(fft, depth);
            }, delegate
            {
                layerNow.childLayer(this,
                        new Rect(layerNow.innerrect.x, layerNow.innerrect.y + (layerNow.innerrect.height / 2f),
                            layerNow.innerrect.width / 2f, layerNow.innerrect.height / 2f), depth)
                    .tagInc("SplitSub", 1).setBackground(fft.getRandomColor("Background", MI(1, depth)))
                    .iterateLayer(fft, depth);
            }, delegate
            {
                layerNow.childLayer(this,
                        new Rect(layerNow.innerrect.x + (layerNow.innerrect.width / 2f),
                            layerNow.innerrect.y + (layerNow.innerrect.height / 2f), layerNow.innerrect.width / 2f,
                            layerNow.innerrect.height / 2f), depth).tagInc("SplitSub", 1)
                    .setBackground(fft.getRandomColor("Background", MI(0, depth)))
                    .iterateLayer(fft, depth);
            });
    }
}