using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Amnabi;

public class Dialog_AssignPurposeToFlag : Window
{
    private const float EntryHeight = 35f;
    private readonly ISettlementAssignableFlag assignable;

    private Vector2 scrollPosition;

    public Dialog_AssignPurposeToFlag(ISettlementAssignableFlag assignable)
    {
        this.assignable = assignable;
        doCloseButton = true;
        doCloseX = true;
        forcePause = true;
        closeOnClickedOutside = false;
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
        var num = assignable.AssignablePlayerFaction.Count();
        var num2 = assignable.AssignableOtherFaction.Count();
        var num3 = assignable.AssignablePlayerSettlement.Count();
        var num4 = assignable.AssignableOtherSettlement.Count();
        var num5 = FlagSettings.modifyOtherFactionsFlags ? num2 + num4 : 0;
        num5 += num + num3;
        var viewRect = new Rect(0f, 0f, outRect.width - 16f,
            (num5 * 35f) + (FlagSettings.modifyOtherFactionsFlags ? 200f : 100f));
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        try
        {
            var curY = 0f;
            if (!assignable.currentFlagID().Equals("TILEID-1"))
            {
                var rect = new Rect(0f, curY, viewRect.width, 32f);
                if (Widgets.ButtonText(rect, "BuildingUnassign".Translate(), true, false))
                {
                    assignable.unassign();
                }

                curY += 35f;
            }
            else
            {
                var rect2 = new Rect(-100f, -100f, 0f, 0f);
                if (!Widgets.ButtonText(rect2, "DEBUGTEXTBOXSYNC", true, false))
                {
                }
            }

            Widgets.ListSeparator(ref curY, viewRect.width, "Amnabi.Faction".Translate());
            foreach (var item in assignable.AssignablePlayerFaction)
            {
                var rect3 = new Rect(0f, curY, viewRect.width * 0.6f, 32f);
                var text = item is Faction faction ? faction.GetCallLabel() : "error";
                text = text.NullOrEmpty() ? "Amnabi.YourUnnamedFaction".Translate() : text;
                Widgets.Label(rect3, text);
                rect3.x = rect3.xMax;
                rect3.width = viewRect.width * 0.4f;
                if (Widgets.ButtonText(rect3, "BuildingAssign".Translate(), true, false))
                {
                    assignable.TryAssignObject(item);
                    Close();
                    break;
                }

                curY += 35f;
            }

            Widgets.ListSeparator(ref curY, viewRect.width, "Amnabi.Settlement".Translate());
            foreach (var item2 in assignable.AssignablePlayerSettlement)
            {
                var rect4 = new Rect(0f, curY, viewRect.width * 0.6f, 32f);
                var label = item2 is Settlement settlement ? settlement.Label : "error";
                Widgets.Label(rect4, label);
                rect4.x = rect4.xMax;
                rect4.width = viewRect.width * 0.4f;
                if (Widgets.ButtonText(rect4, "BuildingAssign".Translate(), true, false))
                {
                    assignable.TryAssignObject(item2);
                    Close();
                    break;
                }

                curY += 35f;
            }

            if (FlagSettings.modifyOtherFactionsFlags)
            {
                Widgets.ListSeparator(ref curY, viewRect.width, "Amnabi.OtherFaction".Translate());
                foreach (var item3 in assignable.AssignableOtherFaction)
                {
                    var rect5 = new Rect(0f, curY, viewRect.width * 0.6f, 32f);
                    var label2 = item3 is Faction faction2 ? faction2.GetCallLabel() : "error";
                    Widgets.Label(rect5, label2);
                    rect5.x = rect5.xMax;
                    rect5.width = viewRect.width * 0.4f;
                    if (Widgets.ButtonText(rect5, "BuildingAssign".Translate(), true, false))
                    {
                        assignable.TryAssignObject(item3);
                        Close();
                        break;
                    }

                    curY += 35f;
                }

                Widgets.ListSeparator(ref curY, viewRect.width, "Amnabi.OtherSettlement".Translate());
                foreach (var item4 in assignable.AssignableOtherSettlement)
                {
                    var rect6 = new Rect(0f, curY, viewRect.width * 0.6f, 32f);
                    var label3 = item4 is Settlement settlement2 ? settlement2.Label : "error";
                    Widgets.Label(rect6, label3);
                    rect6.x = rect6.xMax;
                    rect6.width = viewRect.width * 0.4f;
                    if (Widgets.ButtonText(rect6, "BuildingAssign".Translate(), true, false))
                    {
                        assignable.TryAssignObject(item4);
                        Close();
                        break;
                    }

                    curY += 35f;
                }
            }

            Widgets.ListSeparator(ref curY, viewRect.width, "Amnabi.CustomPurpose".Translate());
            TooltipHandler.TipRegion(new Rect(0f, curY, viewRect.width, 32f), "Amnabi.EnterID.Desc".Translate());
            var rect7 = new Rect(0f, curY, viewRect.width * 0.25f, 32f);
            string label4 = "Amnabi.EnterID".Translate();
            Widgets.Label(rect7, label4);
            rect7.x = rect7.xMax;
            rect7.width = viewRect.width * 0.75f;
            var text2 = Widgets.TextArea(rect7, assignable.currentFlagID());
            if (!text2.Equals(assignable.currentFlagID()))
            {
                assignable.TryAssignObject(text2);
            }
        }
        finally
        {
            Widgets.EndScrollView();
        }
    }
}