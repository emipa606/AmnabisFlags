using UnityEngine;

namespace Amnabi;

public class PR_BiHorizontalSplit : PR_HorizontalSplit
{
    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        ExecRandOrder(delegate
        {
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x, layerNow.innerrect.y, layerNow.innerrect.width,
                        layerNow.innerrect.height / 2f), depth).tagInc("SplitSub", 1).tagInc("SplitSubHori", 1)
                .iterateLayer(fft, depth);
        }, delegate
        {
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x, layerNow.innerrect.y + (layerNow.innerrect.height / 2f),
                        layerNow.innerrect.width, layerNow.innerrect.height / 2f), depth).tagInc("SplitSub", 1)
                .tagInc("SplitSubHori", 1)
                .setBackground(fft.getRandomColor("Background", MI(1, depth)))
                .iterateLayer(fft, depth);
        });
    }
}