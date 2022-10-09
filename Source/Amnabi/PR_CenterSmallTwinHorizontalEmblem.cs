using UnityEngine;

namespace Amnabi;

public class PR_CenterSmallTwinHorizontalEmblem : PR_MultiEmblem
{
    public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        base.iterate(fft, layerNow, depth);
        ExecRandOrder(delegate
        {
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x, layerNow.innerrect.y + (layerNow.innerrect.height / 4f),
                        layerNow.innerrect.width / 2f, layerNow.innerrect.height / 2f), depth)
                .tagInc(EMBLEMBACKMUST, 1).iterateLayer(fft, depth)
                .tagInc(EMBLEMINNERMUST, 1)
                .iterateLayer(fft, depth)
                .tagInc(EMBLEMOVERMUST, 1)
                .iterateLayer(fft, depth);
        }, delegate
        {
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x + (layerNow.innerrect.width / 2f),
                        layerNow.innerrect.y + (layerNow.innerrect.height / 4f), layerNow.innerrect.width / 2f,
                        layerNow.innerrect.height / 2f), depth).tagInc(EMBLEMBACKMUST, 1).iterateLayer(fft, depth)
                .tagInc(EMBLEMINNERMUST, 1)
                .iterateLayer(fft, depth)
                .tagInc(EMBLEMOVERMUST, 1)
                .iterateLayer(fft, depth);
        });
        if (recurseLayer)
        {
            layerNow.iterateLayer(fft, depth);
        }
    }
}