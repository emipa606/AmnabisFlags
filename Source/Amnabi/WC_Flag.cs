using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Amnabi
{
    public class WC_Flag : WorldComponent
    {
        public static List<Flag> popInvalidBeforeSave = new List<Flag>();

        public static List<string> keyIterBeforeSave = new List<string>();

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
                        !Harmony_Flags.prefabricatedFlagDefSort.ContainsKey(allFaction.def))
                    {
                        continue;
                    }

                    var flagPatternDef = Harmony_Flags.prefabricatedFlagDefSort[allFaction.def]
                        .RandomElementByWeight(x => x.probability);
                    if (flagPatternDef == null)
                    {
                        continue;
                    }

                    var flag = FlagsCore.CreateFlagFaction(allFaction);
                    flag.patternStack.Add(new FlagPattern(flagPatternDef));
                    flag.patternStack[flag.patternMax].customURL = null;
                    flag.patternStack[flag.patternMax].alpha = 1f;
                    flag.patternStack[flag.patternMax].red = 1f;
                    flag.patternStack[flag.patternMax].green = 1f;
                    flag.patternStack[flag.patternMax].blue = 1f;
                    flag.patternStack[flag.patternMax].offsetX = FlagsCore.GLOBALFLAGWIDTH / 2f;
                    flag.patternStack[flag.patternMax].offsetY = FlagsCore.GLOBALFLAGHEIGHT / 2f;
                    flag.patternStack[flag.patternMax].scaleX = 1f *
                                                                (flag.patternStack[flag.patternMax]
                                                                     .effectiveTexture.width /
                                                                 (float)flag.patternStack[flag.patternMax]
                                                                     .effectiveTexture.height);
                    flag.patternStack[flag.patternMax].scaleY = 1f;
                    flag.patternStack[flag.patternMax].angle = 0f;
                    flag.patternMax++;
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
            popInvalidBeforeSave.Clear();
            keyIterBeforeSave.Clear();
            foreach (var key in FlagsCore.flagIDToFlag.Keys)
            {
                if (FlagsCore.flagIDToFlag[key].isValidFlag())
                {
                    continue;
                }

                FlagsCore.flagIDToFlag[key].recycle();
                popInvalidBeforeSave.Add(FlagsCore.flagIDToFlag[key]);
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

            popInvalidBeforeSave.Clear();
            keyIterBeforeSave.Clear();
        }

        public FactionFlagTags factionFlagGen_instance(Faction fac)
        {
            var key = fac == null ? "Factionless" : fac.GetUniqueLoadID();
            if (allFactionFlagGenerators.ContainsKey(key))
            {
                return allFactionFlagGenerators[key];
            }

            allFactionFlagGenerators.Add(key, new FactionFlagTags());
            var factionFlagTags = allFactionFlagGenerators[key];
            factionFlagTags.themeTags.Add("Imperial", 0.1);
            factionFlagTags.themeTags.Add("Colonial", 0.2);
            factionFlagTags.initialize(fac);

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
            var flagSettlement = FlagsCore.GetFlagSettlement(stt);
            if (flagSettlement != null)
            {
                return flagSettlement;
            }

            return factionFlagGen(stt.Faction).initialize(stt);
        }
    }
}