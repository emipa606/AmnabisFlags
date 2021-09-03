using UnityEngine;

namespace Amnabi
{
    public class PR_Split : PatternRecursive
    {
        public int MI(int i, int depth)
        {
            return depth == 0 ? i : i;
        }

        public override void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
        {
            base.iterate(fft, layerNow, depth);
            layerNow.tagInc("FlagSplit", 1);
        }

        public override double getProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
        {
            var num = layerNow.tagT("SplitSub");
            var num2 = 1f;
            return num switch
            {
                0 => num2 * 6f,
                1 => num2 * 3f,
                2 => num2 * 1f,
                3 => num2 / 2f,
                4 => num2 / 4f,
                5 => num2 / 10f,
                6 => num2 / 20f,
                _ => 0f
            } / Mathf.Max(1f, num * num);
        }
    }
}