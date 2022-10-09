using Mlie;
using UnityEngine;
using Verse;

namespace Amnabi;

public class FlagMod : Mod
{
    public static FlagSettings settings;

    public static string currentVersion;

    public FlagMod(ModContentPack content)
        : base(content)
    {
        settings = GetSettings<FlagSettings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(ModLister.GetActiveModWithIdentifier("Mlie.AmnabisFlags"));
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        settings.DoWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Amnabi'sFlags";
    }
}