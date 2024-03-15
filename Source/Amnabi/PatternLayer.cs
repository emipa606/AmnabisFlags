using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Amnabi;

public class PatternLayer
{
    private readonly List<PatternLayer> child = [];

    public readonly List<FlagPattern> postFlagPatterns = [];

    public readonly List<FlagPattern> preFlagPatterns = [];

    public readonly Dictionary<string, int> tagOccupationNum = new Dictionary<string, int>();

    public readonly Dictionary<string, int> tagOccupationNumTop = new Dictionary<string, int>();
    public PatternRecursive assigner;

    public Color currentBackgroundColor = new Color(0f, 0f, 0f, 0f);

    public Color currentBackgroundColorN1 = new Color(0f, 0f, 0f, 0f);
    public Rect innerrect;

    public PatternLayer parent;

    public float wRatio()
    {
        return innerrect.height == 0f
            ? 1000f
            : Mathf.Clamp(Mathf.Abs(innerrect.width / innerrect.height), 0f, 1000f);
    }

    public float hRatio()
    {
        return innerrect.width == 0f
            ? 1000f
            : Mathf.Clamp(Mathf.Abs(innerrect.height / innerrect.width), 0f, 1000f);
    }

    public float minRatio()
    {
        if (innerrect.width > innerrect.height)
        {
            return innerrect.height / innerrect.width;
        }

        if (innerrect.width < innerrect.height)
        {
            return innerrect.width / innerrect.height;
        }

        return 1f;
    }

    public float MinDims()
    {
        return Mathf.Min(innerrect.width, innerrect.height);
    }

    public float MaxDims()
    {
        return Mathf.Max(innerrect.width, innerrect.height);
    }

    public PatternLayer setBackgroundColorUpdate(Color c)
    {
        currentBackgroundColorN1 = currentBackgroundColor;
        currentBackgroundColor = c;
        return this;
    }

    public PatternLayer setBackground(Color c)
    {
        attachPre(PatternRecursive.createFlagPattern(AmnabiFlagDefOfs.Pattern_Square, c,
            innerrect.x + (innerrect.width / 2f), innerrect.y + (innerrect.height / 2f), innerrect.width,
            innerrect.height, 0.0));
        setBackgroundColorUpdate(c);
        return this;
    }

    public PatternLayer iterateLayer(FactionFlagTags fft, int depth)
    {
        var patternRecursive = PatternRecursive.pickRandomPattern(fft, this, depth + 1);
        if (patternRecursive == null)
        {
            Log.Warning("Fallback failed, patternRecursive is Null");
            return this;
        }

        patternRecursive.iterate(fft, this, depth + 1);
        return this;
    }

    public PatternLayer childLayer(PatternRecursive auth, Rect rect, int depth)
    {
        var patternLayer = new PatternLayer
        {
            assigner = auth,
            innerrect = rect
        };
        child.Add(patternLayer);
        patternLayer.parent = this;
        patternLayer.currentBackgroundColor = currentBackgroundColor;
        patternLayer.currentBackgroundColorN1 = currentBackgroundColorN1;
        return patternLayer;
    }

    public void attachPre(FlagPattern fp)
    {
        preFlagPatterns.Add(fp);
    }

    public PatternLayer attachPost(FlagPattern fp)
    {
        postFlagPatterns.Add(fp);
        return this;
    }

    public void compile(List<FlagPattern> fp)
    {
        compilePart(fp);
        compilePart(fp, false);
    }

    public void compilePart(List<FlagPattern> fp, bool pre = true)
    {
        if (pre)
        {
            fp.AddRange(preFlagPatterns);
            foreach (var item in child)
            {
                item.compilePart(fp);
            }

            return;
        }

        foreach (var item2 in child)
        {
            item2.compilePart(fp, false);
        }

        fp.AddRange(postFlagPatterns);
    }

    public int tagT(string str)
    {
        return parent?.tagT(str) ?? tagOccupationNumTop.GetValueOrDefault(str, 0);
    }

    public void tagT(string str, int i)
    {
        if (parent != null)
        {
            parent.tagT(str, i);
            return;
        }

        if (tagOccupationNumTop.ContainsKey(str))
        {
            tagOccupationNumTop.Remove(str);
        }

        tagOccupationNumTop.Add(str, i);
    }

    public int tagS(string str)
    {
        var num = 0;
        if (parent != null)
        {
            num = parent.tagS(str);
        }

        if (tagOccupationNum.TryGetValue(str, out var value))
        {
            return value + num;
        }

        return num;
    }

    public int tag(string str)
    {
        return tagOccupationNum.GetValueOrDefault(str, 0);
    }

    public void tag(string str, int i)
    {
        var num = 0;
        if (tagOccupationNum.ContainsKey(str))
        {
            num = tagOccupationNum[str];
            tagOccupationNum.Remove(str);
        }

        tagT(str, tagT(str) + i - num);
        tagOccupationNum.Add(str, i);
    }

    public PatternLayer tagInc(string str, int i)
    {
        tag(str, i + tag(str));
        return this;
    }
}