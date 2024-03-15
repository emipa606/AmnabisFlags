using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Amnabi;

public class PatternRecursive
{
    public const string LONGSTICK = "LongStick";

    public const string TOOL = "Tool";

    public const string ICON = "Icon";

    public const string EMBLEMBASE = "EmblemBase";

    public const string EMBLEMOVER = "EmblemTop";

    public const string BACKGROUND = "Background";

    public static readonly List<PatternRecursive> allPatterns;

    public static readonly float VHBiasNormal;

    public static readonly float probabilityMultiplier;

    public float baseProbabilityV = 1f;

    static PatternRecursive()
    {
        allPatterns = [];
        VHBiasNormal = 1.5f;
        probabilityMultiplier = 1f;
        allPatterns.Add(new PR_TriangularRightSplit
        {
            baseProbability = 0.24f
        });
        allPatterns.Add(new PR_TriangularLeftSplit
        {
            baseProbability = 0.24f
        });
        allPatterns.Add(new PR_TriHorizontalSplit
        {
            baseProbability = 0.24f * VHBiasNormal
        });
        allPatterns.Add(new PR_BiHorizontalSplit
        {
            baseProbability = 0.24f * VHBiasNormal
        });
        allPatterns.Add(new PR_QQHHorizontalSplit
        {
            baseProbability = 0.24f * VHBiasNormal
        });
        allPatterns.Add(new PR_QHQHorizontalSplit
        {
            baseProbability = 0.24f * VHBiasNormal
        });
        allPatterns.Add(new PR_HQQHorizontalSplit
        {
            baseProbability = 0.24f * VHBiasNormal
        });
        allPatterns.Add(new PR_TriVerticalSplit
        {
            baseProbability = 0.24f
        });
        allPatterns.Add(new PR_BiVerticalSplit
        {
            baseProbability = 0.24f
        });
        allPatterns.Add(new PR_QQHVerticalSplit
        {
            baseProbability = 0.24f
        });
        allPatterns.Add(new PR_QHQVerticalSplit
        {
            baseProbability = 0.24f
        });
        allPatterns.Add(new PR_HQQVerticalSplit
        {
            baseProbability = 0.24f
        });
        allPatterns.Add(new PR_QuadSplit
        {
            baseProbability = 0.18f
        });
        allPatterns.Add(new PR_CheckerQuadSplit
        {
            baseProbability = 0.6f
        });
        allPatterns.Add(new PR_NoSplit
        {
            baseProbability = 1f
        });
        allPatterns.Add(new PR_CenterBigEmblem
        {
            baseProbability = 1f
        });
        allPatterns.Add(new PR_CenterSmallEmblem
        {
            baseProbability = 1f
        });
        allPatterns.Add(new PR_CenterSmallTwinVerticalEmblem
        {
            baseProbability = 0.6f
        });
        allPatterns.Add(new PR_CenterSmallTwinHorizontalEmblem
        {
            baseProbability = 0.6f
        });
        allPatterns.Add(new PR_TLCEmblem
        {
            baseProbability = 1f
        });
        allPatterns.Add(new PR_CenterBigEmblem
        {
            baseProbability = 1f,
            recurseLayer = true
        });
        allPatterns.Add(new PR_CenterSmallEmblem
        {
            baseProbability = 1f,
            recurseLayer = true
        });
        allPatterns.Add(new PR_CenterSmallTwinVerticalEmblem
        {
            baseProbability = 0.6f,
            recurseLayer = true
        });
        allPatterns.Add(new PR_CenterSmallTwinHorizontalEmblem
        {
            baseProbability = 0.6f,
            recurseLayer = true
        });
        allPatterns.Add(new PR_TLCEmblem
        {
            baseProbability = 1f,
            recurseLayer = true
        });
        allPatterns.Add(new PR_RandomEmblemBase
        {
            baseProbability = 1f
        });
        allPatterns.Add(new PR_RandomEmblemOver
        {
            baseProbability = 1f
        });
        allPatterns.Add(new PR_TF_2Cross
        {
            baseProbability = 8f
        });
        allPatterns.Add(new PR_TF_3Cross
        {
            baseProbability = 4.5f
        });
        allPatterns.Add(new PR_TF_JucheCross
        {
            baseProbability = 4f
        });
        allPatterns.Add(new PR_TF_FalangeCross
        {
            baseProbability = 1f
        });
        allPatterns.Add(new PR_EndDepth
        {
            baseProbability = 0.1f
        });
        allPatterns.Add(new PatternRecursive
        {
            baseProbability = 0.001f
        });
    }

    public float baseProbability
    {
        get => baseProbabilityV;
        set => baseProbabilityV = value * probabilityMultiplier;
    }

    public static void uanis(List<Action> toJJI, Action a)
    {
        if (a != null)
        {
            toJJI.Add(a);
        }
    }

    protected static void ExecRandOrder(Action action0 = null, Action action1 = null, Action action2 = null,
        Action action3 = null)
    {
        var list = new List<Action>();
        uanis(list, action0);
        uanis(list, action1);
        uanis(list, action2);
        uanis(list, action3);
        while (list.Count > 0)
        {
            var index = (int)(list.Count * Rand.Value);
            list[index]();
            list.RemoveAt(index);
        }
    }

    protected static float IntersectionAdjustedPoints(FlagPatternDef fpd, FactionFlagTags fft)
    {
        var num = 0.1f;
        foreach (var themeTag in fpd.themeTags)
        {
            if (fft.themeTags.TryGetValue(themeTag, out var tag))
            {
                num += (float)tag * 4f;
            }
        }

        return num;
    }

    public static PatternRecursive pickRandomPattern(FactionFlagTags baseF, PatternLayer layerNow, int depth)
    {
        var num = 0f;
        if (allPatterns.Any(x => x.getPreProbability(baseF, layerNow, depth) * x.baseProbability > 0.0))
        {
            foreach (var allPattern in allPatterns)
            {
                num += (float)allPattern.getPreProbability(baseF, layerNow, depth) * allPattern.baseProbability;
            }

            num = Rand.Value * num;
            foreach (var allPattern2 in allPatterns)
            {
                num -= (float)allPattern2.getPreProbability(baseF, layerNow, depth) * allPattern2.baseProbability;
                if (num <= 0f)
                {
                    return allPattern2;
                }
            }
        }

        num = 0f;
        foreach (var allPattern3 in allPatterns)
        {
            num += (float)allPattern3.getProbability(baseF, layerNow, depth) * allPattern3.baseProbability;
        }

        num = Rand.Value * num;
        foreach (var allPattern4 in allPatterns)
        {
            var num2 = (float)allPattern4.getProbability(baseF, layerNow, depth) * allPattern4.baseProbability;
            num -= num2;
            if (num <= 0f)
            {
                return allPattern4;
            }
        }

        return null;
    }

    protected virtual double getPreProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        return 0.0;
    }

    protected virtual double getProbability(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
        return 1.0;
    }

    public virtual void iterate(FactionFlagTags fft, PatternLayer layerNow, int depth)
    {
    }

    public static FlagPattern createFlagPattern(FlagPatternDef fpd, Color c, double x, double y, double sizeX,
        double sizeY, double angleDeg)
    {
        var pattern = new FlagPattern(fpd)
        {
            alpha = c.a,
            red = c.r,
            green = c.g,
            blue = c.b,
            scaleX = (float)sizeX,
            scaleY = (float)sizeY,
            offsetX = (float)x * FlagsCore.GLOBALFLAGHEIGHT,
            offsetY = (float)y * FlagsCore.GLOBALFLAGHEIGHT,
            angle = (float)angleDeg
        };
        return pattern;
    }
}