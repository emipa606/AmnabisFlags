using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Amnabi;

public class WC_Flag : WorldComponent
{
    public static readonly List<Flag> PopInvalidBeforeSave = [];

    public static readonly List<string> keyIterBeforeSave = [];

    public static WC_Flag staticVersion;

    public static Dictionary<string, Flag> flagTempLoader = new Dictionary<string, Flag>();

    public Dictionary<string, FactionFlagTags> allFactionFlagGenerators = new Dictionary<string, FactionFlagTags>();

    public WC_Flag(World world)
        : base(world)
    {
        staticVersion = this;
    }

    public override void FinalizeInit()
    {
        try
        {
            foreach (var allFaction in Find.FactionManager.AllFactions)
            {
                if (FlagsCore.GetFlagFaction(allFaction) != null ||
                    !HarmonyFlags.PrefabricatedFlagDefSort.TryGetValue(allFaction.def, out var value))
                {
                    continue;
                }

                var elementByWeight = value
                    .RandomElementByWeight(x => x.probability);
                if (elementByWeight == null)
                {
                    continue;
                }

                var faction = FlagsCore.CreateFlagFaction(allFaction);
                faction.patternStack.Add(new FlagPattern(elementByWeight));
                faction.patternStack[faction.patternMax].customURL = null;
                faction.patternStack[faction.patternMax].alpha = 1f;
                faction.patternStack[faction.patternMax].red = 1f;
                faction.patternStack[faction.patternMax].green = 1f;
                faction.patternStack[faction.patternMax].blue = 1f;
                faction.patternStack[faction.patternMax].offsetX = FlagsCore.GLOBALFLAGWIDTH / 2f;
                faction.patternStack[faction.patternMax].offsetY = FlagsCore.GLOBALFLAGHEIGHT / 2f;
                faction.patternStack[faction.patternMax].scaleX = 1f *
                                                                  (faction.patternStack[faction.patternMax]
                                                                       .effectiveTexture.width /
                                                                   (float)faction.patternStack[faction.patternMax]
                                                                       .effectiveTexture.height);
                faction.patternStack[faction.patternMax].scaleY = 1f;
                faction.patternStack[faction.patternMax].angle = 0f;
                faction.patternMax++;
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex.Message);
        }
    }

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref allFactionFlagGenerators, "AFFG", LookMode.Value, LookMode.Deep);
        PopInvalidBeforeSave.Clear();
        keyIterBeforeSave.Clear();
        foreach (var key in FlagsCore.flagIDToFlag.Keys)
        {
            if (FlagsCore.flagIDToFlag[key].isValidFlag())
            {
                continue;
            }

            FlagsCore.flagIDToFlag[key].recycle();
            PopInvalidBeforeSave.Add(FlagsCore.flagIDToFlag[key]);
            keyIterBeforeSave.Add(key);
        }

        foreach (var item in keyIterBeforeSave)
        {
            FlagsCore.flagIDToFlag.Remove(item);
        }

        Scribe_Collections.Look(ref FlagsCore.flagIDToFlag, "FIDTF", LookMode.Value, LookMode.Deep);
        if (FlagsCore.flagIDToFlag == null)
        {
            FlagsCore.flagIDToFlag = new Dictionary<string, Flag>();
        }

        PopInvalidBeforeSave.Clear();
        keyIterBeforeSave.Clear();
    }

    public FactionFlagTags factionFlagGen_instance(Faction fac)
    {
        var key = fac == null ? "Factionless" : fac.GetUniqueLoadID();
        if (allFactionFlagGenerators.TryGetValue(key, out var instance))
        {
            return instance;
        }

        allFactionFlagGenerators.Add(key, new FactionFlagTags());
        var generator = allFactionFlagGenerators[key];
        generator.themeTags.Add("Imperial", 0.1);
        generator.themeTags.Add("Colonial", 0.2);
        generator.initialize(fac);

        return allFactionFlagGenerators[key];
    }

    public static FactionFlagTags factionFlagGen(Faction fac)
    {
        return staticVersion.factionFlagGen_instance(fac);
    }

    public static Flag settlementFlagGen(Settlement stt)
    {
        return staticVersion.settlementFlagGen_instance(stt);
    }

    public Flag settlementFlagGen_instance(Settlement stt)
    {
        var settlement = FlagsCore.GetFlagSettlement(stt);
        return settlement ?? factionFlagGen(stt.Faction).initialize(stt);
    }
}