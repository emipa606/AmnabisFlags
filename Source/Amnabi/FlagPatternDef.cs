using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Amnabi;

public class FlagPatternDef : Def
{
    public readonly List<float> availableRandomizedAngles = [];

    public readonly string category = "None";

    public Vector2 generateScale = new Vector2(1f, 1f);

    public float probability = 1f;

    private Texture2D readableTexture;

    public List<string> tags = [];

    private string texture;

    public List<string> themeTags = [];

    [Unsaved] private Texture2D theTexture;

    public FactionDef usedForFaction;

    public Texture2D Pattern
    {
        get
        {
            if (theTexture != null)
            {
                return readableTexture;
            }

            theTexture = ContentFinder<Texture2D>.Get(texture);
            if (theTexture != null)
            {
                readableTexture = FlagsCore.CreateTextureFromBase(theTexture);
            }

            return readableTexture;
        }
    }

    public bool isNormalPattern()
    {
        return !category.Equals("None");
    }

    public float getRandomAngle()
    {
        return availableRandomizedAngles.NullOrEmpty() ? 0f : availableRandomizedAngles.RandomElement();
    }
}