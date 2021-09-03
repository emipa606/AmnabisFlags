using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Amnabi
{
    public class ThemeTag
    {
        public double getProbability(FactionFlagTags fft)
        {
            return 1.0;
        }

        public void generateColours(FactionFlagTags FFT)
        {
            var sigma = new HashSet<Color>();
            var num = 2 + (int)(Rand.Value * 3f);
            for (var i = 0; i < num; i++)
            {
                var color = ColorGeneratorT.colourFrequency.Keys.RandomElementByWeight(x =>
                    (sigma.Contains(x) ? 0.03f : 1f) * (float)ColorGeneratorT.colourFrequency[x]);
                sigma.Add(color);
                color = ColorGeneratorT.variantOf(color);
                FFT.addColorTheme("Background", color);
            }

            for (var j = 0; j < num; j++)
            {
                var color2 = ColorGeneratorT.colourFrequency.Keys.RandomElementByWeight(x =>
                    (sigma.Contains(x) ? 0.03f : 1f) * (float)ColorGeneratorT.colourFrequency[x]);
                sigma.Add(color2);
                color2 = ColorGeneratorT.variantOf(color2);
                FFT.addColorTheme("LongStick", color2);
                FFT.addColorTheme("EmblemBase", color2);
                FFT.addColorTheme("EmblemTop", color2);
            }
        }
    }
}