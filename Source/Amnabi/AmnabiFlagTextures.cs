using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Amnabi
{
    [StaticConstructorOnStartup]
    public static class AmnabiFlagTextures
    {
        public static Dictionary<FlagShape, Material> materialParts = new Dictionary<FlagShape, Material>();

        public static Texture2D Nepal =
            FlagsCore.CreateTextureFromBase(ContentFinder<Texture2D>.Get("FlagShape/Nepal"));

        public static Texture2D Ohio = FlagsCore.CreateTextureFromBase(ContentFinder<Texture2D>.Get("FlagShape/Ohio"));

        public static Texture2D Rectangle15 =
            FlagsCore.CreateTextureFromBase(ContentFinder<Texture2D>.Get("FlagShape/Rectangle15"));

        public static Texture2D Square =
            FlagsCore.CreateTextureFromBase(ContentFinder<Texture2D>.Get("FlagShape/Square"));

        public static Texture2D TriangleDown =
            FlagsCore.CreateTextureFromBase(ContentFinder<Texture2D>.Get("FlagShape/TriangleDown"));

        public static Texture2D TriangleUp =
            FlagsCore.CreateTextureFromBase(ContentFinder<Texture2D>.Get("FlagShape/TriangleUp"));

        public static Texture2D Circle =
            FlagsCore.CreateTextureFromBase(ContentFinder<Texture2D>.Get("FlagShape/Circle"));

        public static Texture2D blankFlag;

        public static readonly Texture2D Copy = ContentFinder<Texture2D>.Get("UI/Buttons/Copy");

        public static readonly Texture2D Paste = ContentFinder<Texture2D>.Get("UI/Buttons/Paste");

        public static readonly Texture2D FlagOverlay =
            FlagsCore.CreateTextureFromBase(ContentFinder<Texture2D>.Get("Flags/FlagOverlay"));

        public static readonly Texture2D DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");

        public static readonly Texture2D thePoleTex = ContentFinder<Texture2D>.Get("Things/FlagPole/FlagPole");

        public static Texture2D textureFrom(FlagShape fShape)
        {
            return fShape switch
            {
                FlagShape.Nepal => Nepal,
                FlagShape.Ohio => Ohio,
                FlagShape.Rectangle15 => Rectangle15,
                FlagShape.Square => Square,
                FlagShape.TriangleDown => TriangleDown,
                FlagShape.TriangleUp => TriangleUp,
                FlagShape.Circle => Circle,
                _ => null
            };
        }

        public static Material getFlagShapeOutline(FlagShape ffs)
        {
            if (!materialParts.ContainsKey(ffs))
            {
                materialParts.Add(ffs,
                    MaterialPool.MatFrom("FlagShape/" + ffs + "_m", ShaderDatabase.Transparent, Color.black));
            }

            return materialParts[ffs];
        }
    }
}