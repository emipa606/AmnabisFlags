using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Amnabi;

public static class ColorGeneratorT
{
    public static Dictionary<Color, double> colourFrequency;

    static ColorGeneratorT()
    {
        colourFrequency = new Dictionary<Color, double>
        {
            { ColorInt(16777215), 1.3 },
            { ColorInt(13504806), 0.25 },
            { ColorInt(9341), 0.3 },
            { ColorInt(0), 0.15 },
            { ColorInt(16568598), 0.25 },
            { ColorInt(38194), 0.25 },
            { ColorInt(11665663), 0.25 },
            { ColorInt(16711790), 0.25 }
        };
    }

    public static float GetContrast(Color a, Color b)
    {
        Color.RGBToHSV(a, out var H, out var S, out var V);
        Color.RGBToHSV(b, out var H2, out var S2, out var V2);
        var num = Mathf.Min(Math.Abs(H - H2), Math.Abs(H + 1f - H2), Math.Abs(H2 + 1f - H));
        var num2 = Math.Abs(S - S2);
        var num3 = Math.Abs(V - V2);
        return num + num2 + num3;
    }

    public static Color ColorInt(int i)
    {
        var result = FlagsCore.ItoF(i);
        result.a = 1f;
        return result;
    }

    public static Color variantOf(Color c)
    {
        var result = c;
        var num = Rand.Value;
        var num2 = Rand.Value;
        var num3 = Rand.Value;
        var value = Rand.Value;
        switch (value)
        {
            case < 0.33f:
                num = num > 0.5f ? 1f - ((1f - num) / 4f) : num / 4f;
                break;
            case < 0.66f:
                num2 = num2 > 0.5f ? 1f - ((1f - num2) / 4f) : num2 / 4f;
                break;
            default:
                num3 = num3 > 0.5f ? 1f - ((1f - num3) / 4f) : num3 / 4f;
                break;
        }

        result.r *= 0.9f + (0.2f * num);
        result.g *= 0.9f + (0.2f * num2);
        result.b *= 0.9f + (0.2f * num3);
        result.r = Math.Min(result.r, 1f);
        result.g = Math.Min(result.g, 1f);
        result.b = Math.Min(result.b, 1f);
        return result;
    }

    public static List<Color> CTpicker(Color c, int generationNum)
    {
        var list = new List<Color> { c };
        Color.RGBToHSV(c, out var H, out _, out _);
        for (var i = 1; i < generationNum; i++)
        {
            var num = H;
            var num2 = 1f;
            num = (num + (i / (float)generationNum)) % 1f;
            var num3 = 0.5f;
            list.Add(Color.HSVToRGB(num, num2, num3));
        }

        return list;
    }

    public static Color HSVRandom()
    {
        var value = Rand.Value;
        var value2 = Rand.Value;
        var value3 = Rand.Value;
        return Color.HSVToRGB(value, value2, value3);
    }

    public static Color HVRandom(float S = 0.8f)
    {
        var value = Rand.Value;
        var value2 = Rand.Value;
        return Color.HSVToRGB(value, S, value2);
    }

    public static Color HRandom(float S = 0.8f, float V = 0.5f)
    {
        var value = Rand.Value;
        return Color.HSVToRGB(value, S, V);
    }
}