using UnityEngine;

namespace Amnabi;

public class PR_TriVerticalSplit : PR_VerticalSplit
{
    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        ExecRandOrder(delegate
        {
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x, layerNow.innerrect.y, layerNow.innerrect.width / 3f,
                        layerNow.innerrect.height), depth).tagInc("SplitSub", 1).tagInc("SplitSubVert", 1)
                .iterateLayer(fft, depth);
        }, delegate
        {
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x + (layerNow.innerrect.width * 1f / 3f), layerNow.innerrect.y,
                        layerNow.innerrect.width / 3f, layerNow.innerrect.height), depth).tagInc("SplitSub", 1)
                .tagInc("SplitSubVert", 1)
                .setBackground(fft.getRandomColor("Background", MI(1, depth)))
                .iterateLayer(fft, depth);
        }, delegate
        {
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x + (layerNow.innerrect.width * 2f / 3f), layerNow.innerrect.y,
                        layerNow.innerrect.width / 3f, layerNow.innerrect.height), depth).tagInc("SplitSub", 1)
                .tagInc("SplitSubVert", 1)
                .setBackground(fft.getRandomColor("Background", MI(2, depth)))
                .iterateLayer(fft, depth);
        });
    }
}