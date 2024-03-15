using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using Verse;

namespace Amnabi;

[StaticConstructorOnStartup]
public static class HarmonyFlags
{
    public static readonly Dictionary<string, HashSet<FlagPatternDef>> PatternByCategory;

    public static readonly Dictionary<string, HashSet<FlagPatternDef>> PatternByTags;

    public static readonly Dictionary<FactionDef, List<FlagPatternDef>> PrefabricatedFlagDefSort;

    public static readonly List<WorldObject> TemporaryWorldObjects;

    static HarmonyFlags()
    {
        PatternByCategory = new Dictionary<string, HashSet<FlagPatternDef>>();
        PatternByTags = new Dictionary<string, HashSet<FlagPatternDef>>();
        PrefabricatedFlagDefSort = new Dictionary<FactionDef, List<FlagPatternDef>>();
        TemporaryWorldObjects = [];
        FlagsCore.CheckLoadedAssemblies();
        FlagsCore.LoadPref();
        ensureFlagTextureLoadedInCurrentThread();
        foreach (var allDef in DefDatabase<FlagPatternDef>.AllDefs)
        {
            if (allDef.usedForFaction == null)
            {
                continue;
            }

            if (!PrefabricatedFlagDefSort.ContainsKey(allDef.usedForFaction))
            {
                PrefabricatedFlagDefSort.Add(allDef.usedForFaction, []);
            }

            PrefabricatedFlagDefSort[allDef.usedForFaction].Add(allDef);
        }

        foreach (var allDef2 in DefDatabase<FlagPatternDef>.AllDefs)
        {
            if (!allDef2.isNormalPattern())
            {
                continue;
            }

            if (!PatternByCategory.ContainsKey(allDef2.category))
            {
                PatternByCategory.Add(allDef2.category, []);
            }

            PatternByCategory[allDef2.category].Add(allDef2);
        }

        foreach (var allDef3 in DefDatabase<FlagPatternDef>.AllDefs)
        {
            foreach (var tag in allDef3.tags)
            {
                if (!PatternByTags.ContainsKey(tag))
                {
                    PatternByTags.Add(tag, []);
                }

                PatternByTags[tag].Add(allDef3);
            }
        }

        var harmony = new Harmony("Amnabi.Flags");
        harmony.PatchAll();
        harmony.Patch(AccessTools.Method(typeof(ExpandableWorldObjectsUtility), "ExpandableWorldObjectsOnGUI"),
            null, new HarmonyMethod(typeof(HarmonyFlags), "ExpandableWorldObjectsOnGUI"));
        harmony.Patch(AccessTools.Method(typeof(SymbolResolver_Settlement), "Resolve"), null,
            new HarmonyMethod(typeof(HarmonyFlags), "GenerateFlags"));
        harmony.Patch(AccessTools.Method(typeof(Pawn_ApparelTracker), "Wear"), null,
            new HarmonyMethod(typeof(HarmonyFlags), "WearPostPatch"));
    }

    public static void ensureFlagTextureLoadedInCurrentThread()
    {
        foreach (var allDef in DefDatabase<FlagPatternDef>.AllDefs)
        {
            _ = allDef.Pattern;
        }
    }

    public static void WearPostPatch(Apparel newApparel)
    {
        try
        {
            newApparel.TryGetComp<CompFlag>()?.AssignFlagFromWearer();
        }
        catch (Exception ex)
        {
            Log.Warning(ex.Message);
        }
    }

    public static void GenerateFlags(ResolveParams rp)
    {
        if (rp.faction == null || FlagsCore.GetFlagFaction(rp.faction) == null)
        {
            return;
        }

        rp.thingRot = Rot4.North;
        BaseGen.symbolStack.Push("amnabiFlags", rp);
    }

    private static void SortByExpandingIconPriority(List<WorldObject> worldObjects)
    {
        worldObjects.SortBy(delegate(WorldObject x)
        {
            var num = x.ExpandingIconPriority;
            if (x.Faction is { IsPlayer: true })
            {
                num += 0.001f;
            }

            return num;
        }, x => x.ID);
    }

    public static void ExpandableWorldObjectsOnGUI()
    {
        if (ExpandableWorldObjectsUtility.TransitionPct == 0f || !FlagSettings.displayFlagOnWorldMap)
        {
            return;
        }

        TemporaryWorldObjects.Clear();
        TemporaryWorldObjects.AddRange(Find.WorldObjects.AllWorldObjects);
        SortByExpandingIconPriority(TemporaryWorldObjects);
        var worldTargeter = Find.WorldTargeter;
        if (worldTargeter.IsTargeting)
        {
            GenWorldUI.WorldObjectsUnderMouse(UI.MousePositionOnUI);
        }

        var settlement = default(Settlement);
        foreach (var tempObject in TemporaryWorldObjects)
        {
            try
            {
                if (!tempObject.def.expandingIcon)
                {
                    continue;
                }

                int num;
                if (!tempObject.HiddenBehindTerrainNow())
                {
                    settlement = tempObject as Settlement;
                    if (settlement != null)
                    {
                        num = !FlagSettings.randomlyGenerateFlagsForAI ||
                              WC_Flag.settlementFlagGen(settlement) != null
                            ? 1
                            : 0;
                        goto IL_00c7;
                    }
                }

                num = 0;
                IL_00c7:
                if (num == 0)
                {
                    continue;
                }

                if (FlagsCore.GetFlagSettlement(settlement) != null)
                {
                    var rect = ExpandableWorldObjectsUtility.ExpandedIconScreenRect(tempObject);
                    rect.xMin -= rect.height / 4f;
                    rect.xMax += rect.height / 4f;
                    rect.y -= rect.height;
                    if (tempObject.ExpandingIconFlipHorizontal)
                    {
                        rect.x = rect.xMax;
                        rect.width *= -1f;
                    }

                    Widgets.DrawTextureRotated(rect,
                        FlagsCore.GetFlagSettlement(settlement).getCompiledFlagTexture(),
                        tempObject.ExpandingIconRotation);
                }
                else if (FlagSettings.displayFactionFlagIfNoSettlementFlagExists &&
                         FlagsCore.GetFlagFaction(settlement.Faction) != null)
                {
                    var rect2 = ExpandableWorldObjectsUtility.ExpandedIconScreenRect(tempObject);
                    rect2.xMin -= rect2.height / 4f;
                    rect2.xMax += rect2.height / 4f;
                    rect2.y -= rect2.height;
                    if (tempObject.ExpandingIconFlipHorizontal)
                    {
                        rect2.x = rect2.xMax;
                        rect2.width *= -1f;
                    }

                    Widgets.DrawTextureRotated(rect2,
                        FlagsCore.GetFlagFaction(settlement.Faction).getCompiledFlagTexture(),
                        tempObject.ExpandingIconRotation);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error while drawing {tempObject.ToStringSafe()}: {ex}");
            }
        }

        TemporaryWorldObjects.Clear();
    }
}