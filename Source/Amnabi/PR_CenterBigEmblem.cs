namespace Amnabi
{
    public class PR_CenterBigEmblem : PR_Emblem
    {
        public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
        {
            base.iterate(fft, layerNow, depth);
            layerNow.childLayer(this, layerNow.innerrect, depth).tagInc(EMBLEMBACKMUST, 1).iterateLayer(fft, depth)
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