namespace Amnabi
{
    public class PR_NoSplit : PR_VerticalSplit
    {
        public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
        {
            base.iterate(fft, layerNow, depth);
            layerNow.tagInc("SplitSub", 1).iterateLayer(fft, depth);
        }
    }
}