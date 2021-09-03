using UnityEngine;
using Verse;

namespace Amnabi
{
    public class FlagMod : Mod
    {
        public static FlagSettings settings;

        public FlagMod(ModContentPack content)
            : base(content)
        {
            settings = GetSettings<FlagSettings>();
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
}