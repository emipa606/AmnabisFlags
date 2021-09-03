using UnityEngine;

namespace Amnabi
{
    public class PR_TLCEmblem : PR_Emblem
    {
        public override double getProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
        {
            return base.getProbability(fft, layerNow, depth) *
                   (layerNow.innerrect.width >= 1f && layerNow.innerrect.height >= 1f ? 3.0 : 0.0);
        }

        public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
        {
            base.iterate(fft, layerNow, depth);
            layerNow.childLayer(this,
                    new Rect(layerNow.innerrect.x, layerNow.innerrect.y + (layerNow.innerrect.height / 2f),
                        layerNow.innerrect.width / 2f, layerNow.innerrect.height / 2f), depth).tagInc(EMBLEMBACKMUST, 1)
                .iterateLayer(fft, depth)
                .tagInc(EMBLEMINNERMUST, 1)
                .iterateLayer(fft, depth)
                .tagInc(EMBLEMOVERMUST, 1)
                .iterateLayer(fft, depth);
            if (recurseLayer)
            {
                layerNow.iterateLayer(fft, depth);
            }
        }
    }
}