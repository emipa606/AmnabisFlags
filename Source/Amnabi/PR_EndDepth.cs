using UnityEngine;

namespace Amnabi
{
    public class PR_EndDepth : PatternRecursive
    {
        public new virtual double getProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
        {
            return Mathf.Pow(3.5f, depth);
        }
    }
}