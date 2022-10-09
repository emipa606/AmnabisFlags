using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Verse;

namespace Amnabi;

public class FlagPattern : IExposable
{
    public static Dictionary<string, Texture2D> stringTexture2D = new Dictionary<string, Texture2D>();

    public float alpha = 1f;

    public float angle;

    public float blue = 0.5f;

    public string customURL;

    public FlagPatternDef flagPatternDef;

    public float green = 0.5f;

    public float offsetX;

    public float offsetY;

    public float red = 0.5f;

    public float scaleX = 1f;

    public float scaleY = 1f;

    public FlagPattern(FlagPatternDef fDef)
    {
        flagPatternDef = fDef;
    }

    public Texture2D effectiveTexture
    {
        get
        {
            if (!customURL.NullOrEmpty())
            {
                return customURLLoad(customURL);
            }

            return flagPatternDef.Pattern;
        }
    }

    public void ExposeData()
    {
        Scribe_Defs.Look(ref flagPatternDef, "RimFlags_Def");
        Scribe_Values.Look(ref offsetX, "RimFlags_offsetX");
        Scribe_Values.Look(ref offsetY, "RimFlags_offsetY");
        Scribe_Values.Look(ref angle, "RimFlags_angle");
        Scribe_Values.Look(ref scaleX, "RimFlags_scaleX", 1f);
        Scribe_Values.Look(ref scaleY, "RimFlags_scaleY", 1f);
        Scribe_Values.Look(ref alpha, "RimFlags_alpha", 1f);
        Scribe_Values.Look(ref green, "RimFlags_green", 1f);
        Scribe_Values.Look(ref blue, "RimFlags_blue", 1f);
        Scribe_Values.Look(ref red, "RimFlags_red", 1f);
        Scribe_Values.Look(ref customURL, "RimFlags_customURL", "");
    }

    public void inheritFromFlagPattern(FlagPattern f)
    {
        flagPatternDef = f.flagPatternDef;
        offsetX = f.offsetX;
        offsetY = f.offsetY;
        angle = f.angle;
        scaleX = f.scaleX;
        scaleY = f.scaleY;
        alpha = f.alpha;
        red = f.red;
        green = f.green;
        blue = f.blue;
        customURL = f.customURL;
    }

    public static Texture2D customURLLoad(string str)
    {
        if (stringTexture2D.ContainsKey(str))
        {
            return stringTexture2D[str];
        }

        var texture2D = LoadPNG(str);
        if (texture2D == null)
        {
            texture2D = AmnabiFlagTextures.DeleteX;
        }

        if (texture2D != null)
        {
            texture2D = FlagsCore.CreateTextureFromBase(texture2D);
        }

        stringTexture2D.Add(str, texture2D);

        return stringTexture2D[str];
    }

    private static Texture2D LoadPNG(string filePath)
    {
        filePath += ".png";
        if (!File.Exists(filePath))
        {
            return null;
        }

        var data = File.ReadAllBytes(filePath);
        var texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, true);
        texture2D.LoadImage(data);
        texture2D.Compress(true);
        texture2D.name = Path.GetFileNameWithoutExtension(filePath);
        texture2D.filterMode = FilterMode.Bilinear;
        texture2D.anisoLevel = 2;
        texture2D.Apply(true, true);

        return texture2D;
    }
}