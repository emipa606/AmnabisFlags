using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Amnabi;

public class Dialog_SaveLoadFlag : Window
{
    private const float EntryHeight = 35f;
    private readonly CompFlag assignable;

    private Vector2 scrollPosition;

    public Dialog_SaveLoadFlag(CompFlag comp)
    {
        assignable = comp;
        doCloseButton = true;
        doCloseX = true;
        closeOnClickedOutside = true;
        absorbInputAroundWindow = true;
    }

    public override Vector2 InitialSize => new Vector2(620f, 500f);

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        var outRect = new Rect(inRect);
        outRect.yMin += 20f;
        outRect.yMax -= 40f;
        outRect.width -= 16f;
        var viewRect = new Rect(0f, 0f, outRect.width - 16f, ((FlagsCore.presets.Count + 1) * 35f) + 100f);
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        try
        {
            var num = 0f;
            if (FlagsCore.presets == null)
            {
                return;
            }

            for (var i = 0; i < FlagsCore.presets.Count; i++)
            {
                var rect = new Rect(0f, num, viewRect.width * 0.6f, 32f);
                TooltipHandler.TipRegion(rect, "Amnabi.Rename".Translate());
                var text = Widgets.TextField(rect, FlagsCore.presets[i].flagName);
                if (!text.Equals(FlagsCore.presets[i].flagName))
                {
                    FlagsCore.presets[i].flagName = text;
                    FlagsCore.SavePref();
                }

                rect.x = (viewRect.width * 0.88f) - 33f;
                rect.width = viewRect.width * 0.06f;
                if (Widgets.ButtonText(rect, "Amnabi.Load".Translate(), true, false))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    if (assignable.flag == null)
                    {
                        assignable.flag = FlagsCore.CreateFlag(assignable.flagID);
                    }

                    assignable.flag.inheritFlag(FlagsCore.presets[i], false);
                    assignable.updateTexture();
                    return;
                }

                rect.x = (viewRect.width * 0.94f) - 33f;
                rect.width = viewRect.width * 0.06f;
                if (Widgets.ButtonText(rect, "Amnabi.Overwrite".Translate(), true, false) &&
                    assignable.flag != null)
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    var name = FlagsCore.presets[i].flagName;
                    FlagsCore.presets[i].inheritFlag(assignable.flag, false);
                    FlagsCore.presets[i].flagName = name;
                    FlagsCore.SavePref();
                    return;
                }

                rect.x = viewRect.width - 33f;
                rect.width = 33f;
                if (Widgets.ButtonImage(rect, AmnabiFlagTextures.DeleteX))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    FlagsCore.presets.RemoveAt(i);
                    FlagsCore.SavePref();
                    return;
                }

                num += 35f;
            }

            if (assignable.flag == null)
            {
                return;
            }

            var rect2 = new Rect(0f, num, viewRect.width * 0.6f, 32f);
            Widgets.Label(rect2, "Amnabi.CurrentFlag".Translate());
            rect2.x = viewRect.width * 0.8f;
            rect2.width = viewRect.width * 0.2f;
            if (Widgets.ButtonText(rect2, "Amnabi.Save".Translate(), true, false))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                new Flag().inheritFlag(assignable.flag, false);
                FlagsCore.presets.Add(new Flag());
                FlagsCore.SavePref();
            }
            else
            {
                rect2.x = viewRect.width * 0.6f;
                rect2.width = viewRect.width * 0.2f;
            }
        }
        finally
        {
            Widgets.EndScrollView();
        }
    }

    public static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
    }
}