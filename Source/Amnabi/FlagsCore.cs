using System;
using System.Collections.Generic;
using System.IO;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Amnabi;

public static class FlagsCore
{
    public static bool FlagsLoaded;

    public static bool SocietyLoaded;

    public static bool randomLoadMode;

    public static Dictionary<string, Flag> flagIDToFlag = new Dictionary<string, Flag>();

    public static List<Flag> presets = new List<Flag>();

    public static readonly string PrefFilePath = Path.Combine(GenFilePaths.ConfigFolderPath, "AmnabisFlags.xml");

    public static bool loaded;

    public static float GLOBALFLAGWIDTH = 384f;

    public static float GLOBALFLAGHEIGHT = 256f;

    public static Texture2D Pattern
    {
        get
        {
            if (AmnabiFlagTextures.blankFlag != null)
            {
                return AmnabiFlagTextures.blankFlag;
            }

            AmnabiFlagTextures.blankFlag = ContentFinder<Texture2D>.Get("Flags/FlagBase");
            if (AmnabiFlagTextures.blankFlag == null)
            {
                Log.Warning("Could not find blank flag");
            }

            return AmnabiFlagTextures.blankFlag;
        }
    }

    private static bool IsLoaded(string str)
    {
        return LoadedModManager.RunningModsListForReading.Any(x => x.Name == str);
    }

    public static void CheckLoadedAssemblies()
    {
        FlagsLoaded = IsLoaded("Amnabi's Flags");
        SocietyLoaded = IsLoaded("Amnabi's Society Core");
    }

    public static Texture2D CreateTextureFromBase(Texture2D baseTex)
    {
        var active = RenderTexture.active;
        var temporary = RenderTexture.GetTemporary(baseTex.width, baseTex.height, 0, RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);
        Graphics.Blit(baseTex, temporary);
        RenderTexture.active = temporary;
        var texture2D = new Texture2D(baseTex.width, baseTex.height, TextureFormat.ARGB32, false);
        texture2D.ReadPixels(new Rect(0f, 0f, temporary.width, temporary.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = active;
        RenderTexture.ReleaseTemporary(temporary);
        return texture2D;
    }

    public static void applyOnTop(Flag flagTex, FlagPattern mask, int mode, bool applyImmed)
    {
        var recycleableTexture = flagTex.GetRecycleableTexture();
        var num = Mathf.Cos((float)Math.PI / 180f * mask.angle);
        var num2 = Mathf.Sin((float)Math.PI / 180f * mask.angle);
        var num3 = 0f - mask.offsetX + (recycleableTexture.flagTextureCompiled.height * mask.scaleX * num / 2f) -
                   (recycleableTexture.flagTextureCompiled.height * mask.scaleY * num2 / 2f);
        var num4 = 0f - mask.offsetY + (recycleableTexture.flagTextureCompiled.height * mask.scaleY * num / 2f) +
                   (recycleableTexture.flagTextureCompiled.height * mask.scaleX * num2 / 2f);
        for (var i = 0; i < recycleableTexture.flagTextureCompiled.height; i++)
        {
            for (var j = 0; j < recycleableTexture.flagTextureCompiled.width; j++)
            {
                var num5 = j + num3;
                var num6 = i + num4;
                var num7 = (int)(((num5 * num) + (num6 * num2)) * mask.effectiveTexture.width / GLOBALFLAGHEIGHT /
                                 mask.scaleX);
                var num8 = (int)(((num6 * num) - (num5 * num2)) * mask.effectiveTexture.height / GLOBALFLAGHEIGHT /
                                 mask.scaleY);
                if (num7 < 0 || num8 < 0 || num7 >= mask.effectiveTexture.width ||
                    num8 >= mask.effectiveTexture.height)
                {
                    continue;
                }

                var pixel = mask.effectiveTexture.GetPixel(num7, num8);
                pixel.a *= mask.alpha;
                pixel.r *= mask.red;
                pixel.g *= mask.green;
                pixel.b *= mask.blue;
                var pixel2 = recycleableTexture.flagTextureCompiled.GetPixel(j, i);
                var color = ItoF(addColours(FtoI(pixel2.a, pixel2.r, pixel2.g, pixel2.b),
                    FtoI(pixel.a, pixel.r, pixel.g, pixel.b), 1.0));
                recycleableTexture.flagTextureCompiled.SetPixel(j, i, color);
            }
        }

        if (applyImmed)
        {
            recycleableTexture.flagTextureCompiled.Apply();
        }
    }

    public static int FtoI(float a, float r, float g, float b)
    {
        return ARGB((int)(a * 255f), (int)(r * 255f), (int)(g * 255f), (int)(b * 255f));
    }

    public static Color ItoF(int c)
    {
        return new Color(R(c) / 255f, G(c) / 255f, B(c) / 255f, A(c) / 255f);
    }

    public static int A(int c)
    {
        return (c >> 24) & 0xFF;
    }

    public static int R(int c)
    {
        return (c >> 16) & 0xFF;
    }

    public static int G(int c)
    {
        return (c >> 8) & 0xFF;
    }

    public static int B(int c)
    {
        return c & 0xFF;
    }

    public static double D2(double c)
    {
        return c * c;
    }

    public static double Dsqr(double c)
    {
        return Math.Sqrt(c);
    }

    public static int ARGB(int a, int r, int g, int b)
    {
        return (a << 24) | (r << 16) | (g << 8) | b;
    }

    public static int addColours(int oC, int nC, double mult)
    {
        var num = A(nC);
        if (num == 0)
        {
            return oC;
        }

        var num2 = A(oC);
        var a = 255 - ((num2 - 255) * (num - 255) / 255);
        var num3 = (255 - num) * num2;
        var num4 = num * 255;
        var num5 = num3 + num4;
        var r = (int)Math.Min(255.0, Math.Round(Dsqr(((D2(R(oC)) * num3) + (D2(R(nC)) * num4)) / num5)));
        var g = (int)Math.Min(255.0, Math.Round(Dsqr(((D2(G(oC)) * num3) + (D2(G(nC)) * num4)) / num5)));
        var b = (int)Math.Min(255.0, Math.Round(Dsqr(((D2(B(oC)) * num3) + (D2(B(nC)) * num4)) / num5)));
        return ARGB(a, r, g, b);
    }

    public static void makeFlagWhite(Flag flagTex, bool applyImmed)
    {
        makeFlagColor(flagTex, Color.white, applyImmed);
    }

    public static void makeFlagColor(Flag flagTex, Color c, bool applyImmed)
    {
        var recycleableTexture = flagTex.GetRecycleableTexture();
        for (var i = 0; i < recycleableTexture.flagTextureCompiled.height; i++)
        {
            for (var j = 0; j < recycleableTexture.flagTextureCompiled.width; j++)
            {
                recycleableTexture.flagTextureCompiled.SetPixel(j, i, c);
            }
        }

        if (applyImmed)
        {
            recycleableTexture.flagTextureCompiled.Apply();
        }
    }

    public static void overlayEffect(Flag flagTex)
    {
        var recycleableTexture = flagTex.GetRecycleableTexture();
        for (var i = 0; i < recycleableTexture.flagTextureCompiled.height; i++)
        {
            for (var j = 0; j < recycleableTexture.flagTextureCompiled.width; j++)
            {
                recycleableTexture.flagTextureCompiled.SetPixel(j, i,
                    recycleableTexture.flagTextureCompiled.GetPixel(j, i) *
                    AmnabiFlagTextures.FlagOverlay.GetPixel(j, i));
            }
        }
    }

    public static void cutOutEffect(Flag flagTex, Texture2D cropOut)
    {
        var recycleableTexture = flagTex.GetRecycleableTexture();
        for (var i = 0; i < recycleableTexture.flagTextureCompiled.height; i++)
        {
            for (var j = 0; j < recycleableTexture.flagTextureCompiled.width; j++)
            {
                recycleableTexture.flagTextureCompiled.SetPixel(j, i,
                    1f - cropOut.GetPixel(j, i).a == 0f
                        ? ItoF(0)
                        : recycleableTexture.flagTextureCompiled.GetPixel(j, i));
            }
        }
    }

    public static void LoadPref()
    {
        if (loaded)
        {
            return;
        }

        loaded = true;
        if (!File.Exists(PrefFilePath))
        {
            Log.Message($"{PrefFilePath} is not found.");
            return;
        }

        try
        {
            Scribe.loader.InitLoading(PrefFilePath);
            try
            {
                randomLoadMode = true;
                ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.None, true);
                var list = new List<Flag>();
                Scribe_Collections.Look(ref list, "presets", LookMode.Deep);
                presets = list;
                Scribe.loader.FinalizeLoading();
                randomLoadMode = false;
            }
            catch
            {
                randomLoadMode = false;
                Scribe.ForceStop();
                throw;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Exception loading FlagPreset: {ex}");
            presets = new List<Flag>();
            randomLoadMode = false;
            Scribe.ForceStop();
        }

        randomLoadMode = false;
    }

    public static void SavePref()
    {
        try
        {
            SafeSaver.Save(PrefFilePath, "FlagPresets", delegate
            {
                ScribeMetaHeaderUtility.WriteMetaHeader();
                var list = presets;
                Scribe_Collections.Look(ref list, "presets", LookMode.Deep);
            });
        }
        catch (Exception ex)
        {
            Log.Error($"Exception while saving world: {ex}");
        }
    }

    public static Flag GetFlagFaction(Faction str)
    {
        return GetFlag($"FACTIONID{str.loadID}");
    }

    public static Flag GetFlagSettlement(Settlement str)
    {
        return GetFlag($"TILEID{str.Tile}");
    }

    public static Flag GetFlag(string str, bool invalidToo = false)
    {
        if (!flagIDToFlag.ContainsKey(str))
        {
            return null;
        }

        if (!invalidToo && !flagIDToFlag[str].isValidFlag())
        {
            return null;
        }

        return flagIDToFlag[str];
    }

    public static Flag CreateFlagFaction(Faction str)
    {
        return CreateFlag($"FACTIONID{str.loadID}");
    }

    public static Flag CreateFlagSettlement(Settlement str)
    {
        return CreateFlag($"TILEID{str.Tile}");
    }

    public static Flag CreateFlag(string str)
    {
        if (!flagIDToFlag.ContainsKey(str))
        {
            flagIDToFlag[str] = new Flag
            {
                flagReadID = str,
                randID = GC_Flag.instance.instanceID
            };
        }
        else if (!flagIDToFlag[str].isValidFlag())
        {
            flagIDToFlag[str].clearFlag();
            flagIDToFlag[str].flagReadID = str;
            flagIDToFlag[str].randID = GC_Flag.instance.instanceID;
        }

        return flagIDToFlag[str];
    }
}