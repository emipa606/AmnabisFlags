using RimWorld;
using UnityEngine;
using Verse;

namespace Amnabi;

public class Dialog_URLInput : Window
{
    public readonly CompFlag thisCompFlag;
    protected string curName;

    private bool focusedRenameField;

    private int startAcceptingInputAtFrame;

    public Dialog_URLInput(CompFlag comp)
    {
        thisCompFlag = comp;
        forcePause = true;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnAccept = false;
        closeOnClickedOutside = true;
    }

    private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

    public override Vector2 InitialSize => new Vector2(600f, 175f);

    public void WasOpenedByHotkey()
    {
        startAcceptingInputAtFrame = Time.frameCount + 1;
    }

    protected virtual AcceptanceReport NameIsValid(string name)
    {
        return name.Length != 0;
    }

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        var returnPress = false;
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
        {
            returnPress = true;
            Event.current.Use();
        }

        GUI.SetNextControlName("RenameField");
        var text = Widgets.TextField(new Rect(0f, 15f, inRect.width, 35f), curName);
        if (AcceptsInput)
        {
            curName = text;
        }
        else
        {
            ((TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl)).SelectAll();
        }

        if (!focusedRenameField)
        {
            UI.FocusControl("RenameField", this);
            focusedRenameField = true;
        }

        if (!(Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK") ||
              returnPress))
        {
            return;
        }

        var acceptanceReport = NameIsValid(curName);
        if (!acceptanceReport.Accepted)
        {
            if (acceptanceReport.Reason.NullOrEmpty())
            {
                Messages.Message("NameIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
            }
            else
            {
                Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, false);
            }

            return;
        }

        if (thisCompFlag.flag == null)
        {
            thisCompFlag.flag = FlagsCore.CreateFlag(thisCompFlag.flagID);
        }

        if (thisCompFlag.flag.patternStack.Count > thisCompFlag.flag.patternMax)
        {
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].customURL = curName;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].flagPatternDef = null;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].alpha = 1f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].red = 1f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].green = 1f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].blue = 1f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].offsetX = FlagsCore.GLOBALFLAGHEIGHT / 2f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].offsetY = FlagsCore.GLOBALFLAGHEIGHT / 2f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].scaleX = 1f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].scaleY = 1f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].angle = 0f;
        }
        else
        {
            thisCompFlag.flag.patternStack.Add(new FlagPattern(null));
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].customURL = curName;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].offsetX = FlagsCore.GLOBALFLAGHEIGHT / 2f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].offsetY = FlagsCore.GLOBALFLAGHEIGHT / 2f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].red = 1f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].green = 1f;
            thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].blue = 1f;
        }

        var effectiveTexture = thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].effectiveTexture;
        var num = effectiveTexture.width / (float)effectiveTexture.height;
        thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].offsetX *= num;
        thisCompFlag.flag.patternStack[thisCompFlag.flag.patternMax].scaleX *= num;
        thisCompFlag.flag.patternMax++;
        Find.WindowStack.TryRemove(this);
    }
}