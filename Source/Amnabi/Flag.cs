using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Amnabi
{
    public class Flag : IExposable, ILoadReferenceable
    {
        public Color backgroundColor = Color.white;

        public string flagName = "Flag";

        public string flagReadID = "TILEID-1";

        public FlagShape flagShape = FlagShape.Rectangle15;

        public bool isRecycled;

        public int patternMax;

        public List<FlagPattern> patternStack = new List<FlagPattern>();

        public int randID;

        public RecycleableTexture recyclableTexture;

        public Flag()
        {
            randID = Rand.Int;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref flagShape, "RimFlags_flagShape");
            Scribe_Values.Look(ref patternMax, "RimFlags_patternMax");
            Scribe_Collections.Look(ref patternStack, "RimFlags_Stack", LookMode.Deep, new object[1]);
            randID = GC_Flag.instance != null ? GC_Flag.instance.instanceID : 0;
            Scribe_Values.Look(ref flagReadID, "RimFlags_flagReadID", "TILEID-1");
            Scribe_Values.Look(ref flagName, "RimFlags_name", "Flag");
            Scribe_Values.Look(ref backgroundColor, "RimFlags_backgroundColor", Color.white);
        }

        public string GetUniqueLoadID()
        {
            if (FlagsCore.randomLoadMode && (flagReadID == null || flagReadID.Equals("TILEID-1")))
            {
                flagReadID = "RANDOMIZEDSLID" + GetHashCode() + "_U_" + Rand.Int;
            }

            return flagReadID;
        }

        public void compileFlag()
        {
            var recycleableTexture = GetRecycleableTexture();
            if (recycleableTexture.flagTextureCompiled == null)
            {
                recycleableTexture.flagTextureCompiled = FlagsCore.CreateTextureFromBase(FlagsCore.Pattern);
            }

            FlagsCore.makeFlagColor(this, backgroundColor, false);
            for (var i = 0; i < patternMax; i++)
            {
                FlagsCore.applyOnTop(this, patternStack[i], 0, false);
            }

            if (FlagSettings.applyCreaseOnFlag)
            {
                FlagsCore.overlayEffect(this);
            }

            if (flagShape != 0)
            {
                FlagsCore.cutOutEffect(this, AmnabiFlagTextures.textureFrom(flagShape));
            }

            recycleableTexture.flagTextureCompiled.Apply();
        }

        public void clearFlag()
        {
            patternMax = 0;
        }

        public void inheritFlag(Flag k, bool inheritRID)
        {
            patternMax = k.patternMax;
            for (var i = 0; i < patternMax; i++)
            {
                if (patternStack.Count > i)
                {
                    patternStack[i].inheritFromFlagPattern(k.patternStack[i]);
                    continue;
                }

                patternStack.Add(new FlagPattern(k.patternStack[i].flagPatternDef));
                patternStack[i].inheritFromFlagPattern(k.patternStack[i]);
            }

            if (inheritRID)
            {
                randID = k.randID;
            }

            flagName = k.flagName;
        }

        public bool isValidFlag()
        {
            return randID == GC_Flag.instance.instanceID;
        }

        public RecycleableTexture GetRecycleableTexture()
        {
            if (recyclableTexture != null)
            {
                return recyclableTexture;
            }

            recyclableTexture = RecycleableTexture.nextRecycleableTexture();
            recyclableTexture.needsRefreshNextCall = true;

            return recyclableTexture;
        }

        public Material getFlagMaterial()
        {
            var recycleableTexture = GetRecycleableTexture();
            if (recycleableTexture.materialCompiiled != null && !recycleableTexture.needsRefreshNextCall)
            {
                return recycleableTexture.materialCompiiled;
            }

            recycleableTexture.materialCompiiled = MaterialPool.MatFrom(getCompiledFlagTexture(),
                FlagSettings.renderPlantWave ? ShaderDatabase.TransparentPlant : ShaderDatabase.Transparent,
                Color.white);
            recycleableTexture.materialCompiiled.mainTexture = getCompiledFlagTexture();

            return recycleableTexture.materialCompiiled;
        }

        public Texture2D getCompiledFlagTexture()
        {
            var recycleableTexture = GetRecycleableTexture();
            if (recycleableTexture.flagTextureCompiled != null && !recycleableTexture.needsRefreshNextCall)
            {
                return recycleableTexture.flagTextureCompiled;
            }

            recycleableTexture.needsRefreshNextCall = false;
            compileFlag();

            return recycleableTexture.flagTextureCompiled;
        }

        public void recycle()
        {
            if (isRecycled)
            {
                Log.Warning("Flag already recycled!");
                return;
            }

            isRecycled = true;
            if (recyclableTexture != null)
            {
                RecycleableTexture.recyclableStatic.Add(recyclableTexture);
            }
        }
    }
}