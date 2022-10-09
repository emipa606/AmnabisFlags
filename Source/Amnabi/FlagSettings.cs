using UnityEngine;
using Verse;

namespace Amnabi;

public class FlagSettings : ModSettings
{
    public static bool applyCreaseOnFlag = true;

    public static bool modifyOtherFactionsFlags;

    public static bool displayFlagOnWorldMap = true;

    public static bool displayFactionFlagIfNoSettlementFlagExists = true;

    public static bool randomlyGenerateFlagsForAI;

    public static bool renderStatic;

    public static bool renderPlantWave;

    public static bool renderFancyWave = true;

    public static int renderFancyWaveInt = 10;

    public static bool use3DBeltBanner;

    public static bool swayBeltBanners = true;

    public static bool useFlagOutline;

    public static bool useFlagOutlineBelt = true;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref applyCreaseOnFlag, "applyCreaseOnFlag", true);
        Scribe_Values.Look(ref modifyOtherFactionsFlags, "modifyOtherFactionsFlags");
        Scribe_Values.Look(ref displayFlagOnWorldMap, "displayFlagOnWorldMap", true);
        Scribe_Values.Look(ref displayFactionFlagIfNoSettlementFlagExists,
            "displayFactionFlagIfNoSettlementFlagExists", true);
        Scribe_Values.Look(ref randomlyGenerateFlagsForAI, "randomlyGenerateFlagsForAI");
        Scribe_Values.Look(ref renderStatic, "renderStatic");
        Scribe_Values.Look(ref renderPlantWave, "renderPlantWave");
        Scribe_Values.Look(ref renderFancyWave, "renderFancyWave", true);
        Scribe_Values.Look(ref use3DBeltBanner, "use3DBeltBanner");
        Scribe_Values.Look(ref swayBeltBanners, "swayBeltBanners", true);
        Scribe_Values.Look(ref useFlagOutline, "useFlagOutline", true);
        Scribe_Values.Look(ref useFlagOutlineBelt, "useFlagOutlineBelt", true);
    }

    public void DoWindowContents(Rect inRect)
    {
        var unused = Current.Game?.CurrentMap;
        var listing_Standard = new Listing_Standard
        {
            ColumnWidth = inRect.width - 34f
        };
        listing_Standard.Begin(inRect);
        listing_Standard.Gap(16f);
        listing_Standard.Label("Amnabi.OptionWarning".Translate());
        listing_Standard.CheckboxLabeled("Amnabi.ApplyCreaseOnFlag".Translate(), ref applyCreaseOnFlag);
        listing_Standard.CheckboxLabeled("Amnabi.ModifyOtherFactionFlags".Translate(), ref modifyOtherFactionsFlags,
            "Amnabi.ModifyOtherFactionFlags.Desc");
        listing_Standard.CheckboxLabeled("Amnabi.DisplayFlagOnWorldMap".Translate(), ref displayFlagOnWorldMap);
        listing_Standard.CheckboxLabeled("Amnabi.DisplayFactionFlagIfNoSettlementFlagExists".Translate(),
            ref displayFactionFlagIfNoSettlementFlagExists);
        listing_Standard.CheckboxLabeled("Amnabi.renderStatic".Translate(), ref renderStatic);
        if (renderStatic)
        {
            renderPlantWave = false;
            renderFancyWave = false;
        }

        listing_Standard.CheckboxLabeled("Amnabi.renderPlantWave".Translate(), ref renderPlantWave);
        if (renderPlantWave)
        {
            renderStatic = false;
            renderFancyWave = false;
        }

        listing_Standard.CheckboxLabeled("Amnabi.renderFancyWave".Translate(), ref renderFancyWave);
        if (renderFancyWave)
        {
            renderPlantWave = false;
            renderStatic = false;
        }

        listing_Standard.CheckboxLabeled("Amnabi.use3DBeltBanner".Translate(), ref use3DBeltBanner);
        listing_Standard.CheckboxLabeled("Amnabi.swayBeltBanners".Translate(), ref swayBeltBanners);
        listing_Standard.CheckboxLabeled("Amnabi.useFlagOutline".Translate(), ref useFlagOutline);
        listing_Standard.CheckboxLabeled("Amnabi.useFlagOutlineBelt".Translate(), ref useFlagOutlineBelt);
        if (FlagMod.currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("Amnabi.modVersion".Translate(FlagMod.currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }
}