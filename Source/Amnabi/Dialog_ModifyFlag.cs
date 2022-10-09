using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Amnabi;

public class Dialog_ModifyFlag : Window
{
    private const float EntryHeight = 35f;
    public static bool ARM = true;

    public static Color guiBackground = new Color(7f / 85f, 5f / 51f, 29f / 255f, 1f);

    public static bool cantonized;

    public static List<FlagPatternDef> staticFlagPatterns;

    public static Colorbar3 colorbar3 = new Colorbar3();

    private readonly CompFlag assignable;

    public int fiddlerIndex;

    public Vector2 flagDimension;

    public int lastMouseState;

    public float lastMouseX;

    public float lastMouseY;

    public bool needsSave;

    private Vector2 scrollPosition;

    public Dialog_ModifyFlag(CompFlag assignable, Vector2 dimension)
    {
        this.assignable = assignable;
        doCloseButton = true;
        doCloseX = true;
        forcePause = true;
        closeOnClickedOutside = false;
        absorbInputAroundWindow = true;
        flagDimension = dimension;
    }

    public override Vector2 InitialSize => new Vector2(620f, 800f);

    public override void Close(bool doCloseSound = true)
    {
        if (needsSave)
        {
            needsSave = false;
            assignable.updateTexture();
        }

        base.Close(doCloseSound);
    }

    public static bool MouseInRect(Rect rect)
    {
        var mousePosition = Event.current.mousePosition;
        var num = mousePosition.x - rect.x;
        var num2 = mousePosition.y - rect.y;
        return num >= 0f && num2 >= 0f && num <= rect.width && num2 <= rect.height;
    }

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        var outRect = new Rect(inRect);
        outRect.yMin += 20f;
        outRect.yMax -= 40f;
        outRect.width -= 16f;
        var num = outRect.width - 16f;
        var num2 = num * 2f / 3f;
        outRect.yMin += num2;
        outRect.yMin += 20f;
        if (assignable.flag != null)
        {
            if (ARM)
            {
                if (Widgets.ButtonImage(new Rect(0f, 20f, num, num2), BaseContent.WhiteTex,
                        assignable.flag.backgroundColor, assignable.flag.backgroundColor, false))
                {
                    lastMouseState = 1 - lastMouseState;
                    var mousePosition = Event.current.mousePosition;
                    lastMouseX = mousePosition.x;
                    lastMouseY = mousePosition.y;
                }

                var matrix = GUI.matrix;
                for (var i = 0; i < assignable.flag.patternMax; i++)
                {
                    var color = new Color(assignable.flag.patternStack[i].red,
                        assignable.flag.patternStack[i].green, assignable.flag.patternStack[i].blue,
                        assignable.flag.patternStack[i].alpha);
                    var offsetX = assignable.flag.patternStack[i].offsetX;
                    var offsetY = assignable.flag.patternStack[i].offsetY;
                    var vector = new Vector3((0f - num2) * Mathf.Abs(assignable.flag.patternStack[i].scaleX) / 2f,
                        (0f - num2) * Mathf.Abs(assignable.flag.patternStack[i].scaleY) / 2f, 0f);
                    var num3 = Mathf.Cos((float)(assignable.flag.patternStack[i].angle * Math.PI / -180.0));
                    var num4 = Mathf.Sin((float)(assignable.flag.patternStack[i].angle * Math.PI / -180.0));
                    var vector2 = new Vector3((vector.x * num3) - (vector.y * num4),
                        (((0f - vector.x) * num4) - (vector.y * num3)) * -1f);
                    GUI.matrix = Matrix4x4.identity *
                                 Matrix4x4.Scale(new Vector3(Prefs.UIScale, Prefs.UIScale, 1f)) *
                                 Matrix4x4.Translate((new Vector3(0f - windowRect.x - Margin,
                                     -20f - windowRect.y - Margin, 0f) * -1f) + vector2) *
                                 Matrix4x4.Translate(new Vector3(0f + (num2 * offsetX / FlagsCore.GLOBALFLAGHEIGHT),
                                     num2 - (num2 * offsetY / FlagsCore.GLOBALFLAGHEIGHT), 0f)) *
                                 Matrix4x4.Rotate(Quaternion.AngleAxis(0f - assignable.flag.patternStack[i].angle,
                                     Vector3.forward)) * Matrix4x4.Translate(new Vector3(0f - windowRect.x - Margin,
                                     0f - windowRect.y - Margin, 0f) * 1f);
                    GUI.color = color;
                    if (i == fiddlerIndex && assignable.flag.patternMax > 1 && lastMouseState == 1)
                    {
                    }

                    GUI.DrawTexture(
                        new Rect(
                            assignable.flag.patternStack[i].scaleX > 0f
                                ? 0f
                                : (0f - num2) * assignable.flag.patternStack[i].scaleX,
                            assignable.flag.patternStack[i].scaleY > 0f
                                ? 0f
                                : (0f - num2) * assignable.flag.patternStack[i].scaleY,
                            num2 * assignable.flag.patternStack[i].scaleX,
                            num2 * assignable.flag.patternStack[i].scaleY),
                        assignable.flag.patternStack[i].effectiveTexture);
                    GUI.color = Color.white;
                }

                GUI.matrix = matrix;
                var rect = inRect.ExpandedBy(Margin);
                GUI.color = guiBackground;
                GUI.DrawTexture(new Rect(0f - Margin, 0f - Margin, rect.width, 20f + Margin), BaseContent.WhiteTex);
                GUI.DrawTexture(new Rect(0f - Margin, 0f - Margin, Margin, rect.height), BaseContent.WhiteTex);
                GUI.DrawTexture(new Rect(inRect.width, 0f - Margin, Margin, rect.height), BaseContent.WhiteTex);
                GUI.DrawTexture(new Rect(0f - Margin, num2 + 20f, rect.width, rect.height - num2 - 20f - Margin),
                    BaseContent.WhiteTex);
                GUI.DrawTexture(new Rect(0f, 20f, num, num2),
                    AmnabiFlagTextures.textureFrom(assignable.flag.flagShape));
                GUI.color = Color.white;
            }
            else if (Widgets.ButtonImage(new Rect(0f, 20f, num, num2), assignable.flag.getCompiledFlagTexture(),
                         Color.white, Color.white, false))
            {
                lastMouseState = 1 - lastMouseState;
                var mousePosition2 = Event.current.mousePosition;
                lastMouseX = mousePosition2.x;
                lastMouseY = mousePosition2.y;
            }

            if (fiddlerIndex >= 0 && fiddlerIndex < assignable.flag.patternStack.Count)
            {
                var num5 = Math.Abs(assignable.flag.patternStack[fiddlerIndex].scaleX);
                var num6 = GUI.HorizontalSlider(new Rect(0f, 0f, num - 20f, 20f), num5, 0f, 1.6f);
                if (num5 != num6)
                {
                    assignable.flag.patternStack[fiddlerIndex].scaleX =
                        num6 * (!(assignable.flag.patternStack[fiddlerIndex].scaleX < 0f) ? 1 : -1);
                    if (ARM)
                    {
                        needsSave = true;
                    }
                    else
                    {
                        assignable.updateTexture();
                    }
                }

                num5 = Math.Abs(assignable.flag.patternStack[fiddlerIndex].scaleY);
                num6 = GUI.VerticalSlider(new Rect(num, 20f, 20f, num2 - 20f), num5, 0f, 1.1f);
                if (num5 != num6)
                {
                    assignable.flag.patternStack[fiddlerIndex].scaleY =
                        num6 * (!(assignable.flag.patternStack[fiddlerIndex].scaleY < 0f) ? 1 : -1);
                    if (ARM)
                    {
                        needsSave = true;
                    }
                    else
                    {
                        assignable.updateTexture();
                    }
                }

                num5 = assignable.flag.patternStack[fiddlerIndex].angle;
                num6 = GUI.HorizontalSlider(new Rect(0f, 20f + num2, num, 20f), num5, -180f, 180f);
                if (num5 != num6)
                {
                    assignable.flag.patternStack[fiddlerIndex].angle = num6;
                    if (ARM)
                    {
                        needsSave = true;
                    }
                    else
                    {
                        assignable.updateTexture();
                    }
                }

                TooltipHandler.TipRegion(new Rect(0f, 20f, num, num2),
                    $"X{assignable.flag.patternStack[fiddlerIndex].offsetX} Y{assignable.flag.patternStack[fiddlerIndex].offsetY}");
                if (Widgets.ButtonText(new Rect(num - 20f, 0f, 20f, 20f),
                        assignable.flag.patternStack[fiddlerIndex].scaleX < 0f ? "-X" : "X"))
                {
                    assignable.flag.patternStack[fiddlerIndex].scaleX =
                        0f - assignable.flag.patternStack[fiddlerIndex].scaleX;
                }

                if (Widgets.ButtonText(new Rect(num, 0f, 20f, 20f),
                        assignable.flag.patternStack[fiddlerIndex].scaleY < 0f ? "-Y" : "Y"))
                {
                    assignable.flag.patternStack[fiddlerIndex].scaleY =
                        0f - assignable.flag.patternStack[fiddlerIndex].scaleY;
                }

                TooltipHandler.TipRegion(new Rect(num - 20f, 0f, 20f, 20f), "Amnabi.XFLIP".Translate());
                TooltipHandler.TipRegion(new Rect(num, 0f, 20f, 20f), "Amnabi.YFLIP".Translate());
            }

            if (lastMouseState == 1)
            {
                var mousePosition3 = Event.current.mousePosition;
                var num7 = mousePosition3.x - lastMouseX;
                var num8 = mousePosition3.y - lastMouseY;
                lastMouseX = mousePosition3.x;
                lastMouseY = mousePosition3.y;
                if (fiddlerIndex >= 0 && fiddlerIndex < assignable.flag.patternStack.Count)
                {
                    assignable.flag.patternStack[fiddlerIndex].offsetX += FlagsCore.GLOBALFLAGHEIGHT * num7 / num2;
                    assignable.flag.patternStack[fiddlerIndex].offsetY -= FlagsCore.GLOBALFLAGHEIGHT * num8 / num2;
                    if (ARM)
                    {
                        needsSave = true;
                    }
                    else
                    {
                        assignable.updateTexture();
                    }
                }
            }
        }

        var viewRect = new Rect(0f, 0f, outRect.width - 16f,
            ((float)(Harmony_Flags.patternByCategory.Keys.Count + 1) / 4 * 35f) +
            ((1 + (assignable.flag?.patternMax ?? 0)) * 35f) + 100f + 50f + 35f);
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        try
        {
            var num9 = 0f;
            if (assignable.flag is { patternMax: > 0 })
            {
                var rect2 = new Rect(0f, num9, viewRect.width * 0.5f, 32f);
                Widgets.Label(rect2, "Amnabi.BackgroundColor".Translate());
                var color2 = assignable.flag.backgroundColor;
                rect2 = new Rect(0f, num9, viewRect.width * 0.6f, 32f)
                {
                    x = viewRect.width * 0.5f,
                    width = viewRect.width * 0.5f
                };
                TooltipHandler.TipRegion(rect2,
                    $"R{(int)(color2.r * 255f)} G{(int)(color2.g * 255f)} B{(int)(color2.b * 255f)}");
                if (colorbar3.SelectColor(rect2, ref color2))
                {
                    assignable.flag.backgroundColor.a = 1f;
                    assignable.flag.backgroundColor.r = color2.r;
                    assignable.flag.backgroundColor.g = color2.g;
                    assignable.flag.backgroundColor.b = color2.b;
                    if (ARM)
                    {
                        needsSave = true;
                    }
                    else
                    {
                        assignable.updateTexture();
                    }
                }

                num9 += 35f;
                for (var j = 0; j < assignable.flag.patternMax; j++)
                {
                    var rect3 = new Rect(0f, num9, viewRect.width, 32f);
                    if (Event.current.type == EventType.MouseDown && MouseInRect(rect3))
                    {
                        fiddlerIndex = j;
                    }

                    if (fiddlerIndex == j)
                    {
                        GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.4f);
                        GUI.DrawTexture(rect3, BaseContent.WhiteTex);
                        GUI.color = Color.white;
                    }

                    var reassign = j;
                    var rect4 = new Rect(0f, num9, (viewRect.width * 0.28f) - 33f, 32f);
                    if (assignable.flag.patternStack[j].flagPatternDef != null)
                    {
                        if (Widgets.ButtonText(rect4,
                                assignable.flag.patternStack[j].flagPatternDef.defName.Translate(), true, false))
                        {
                            FloatMenuUtility.MakeMenu(
                                DefDatabase<FlagPatternDef>.AllDefs.Where(x => x.isNormalPattern()),
                                fpd => fpd.defName.Translate(), fpd => delegate
                                {
                                    assignable.flag.patternStack[reassign].customURL = "";
                                    assignable.flag.patternStack[reassign].flagPatternDef = fpd;
                                    if (ARM)
                                    {
                                        needsSave = true;
                                    }
                                    else
                                    {
                                        assignable.updateTexture();
                                    }
                                });
                        }
                    }
                    else if (!assignable.flag.patternStack[j].customURL.NullOrEmpty())
                    {
                        var label = "ERRORNAME";
                        try
                        {
                            label = Path.GetFileName(assignable.flag.patternStack[j].customURL);
                        }
                        catch
                        {
                            // ignored
                        }

                        Widgets.Label(rect4, label);
                    }

                    var color3 = new Color(assignable.flag.patternStack[j].red,
                        assignable.flag.patternStack[j].green, assignable.flag.patternStack[j].blue,
                        assignable.flag.patternStack[j].alpha);
                    rect4 = new Rect(0f, num9, viewRect.width * 0.6f, 32f)
                    {
                        x = (viewRect.width * 0.64f) - 33f,
                        width = viewRect.width * 0.24f
                    };
                    TooltipHandler.TipRegion(rect4,
                        $"R{(int)(assignable.flag.patternStack[j].red * 255f)} G{(int)(assignable.flag.patternStack[j].green * 255f)} B{(int)(assignable.flag.patternStack[j].blue * 255f)}");
                    if (colorbar3.SelectColor(rect4, ref color3))
                    {
                        assignable.flag.patternStack[j].alpha = 1f;
                        assignable.flag.patternStack[j].red = color3.r;
                        assignable.flag.patternStack[j].green = color3.g;
                        assignable.flag.patternStack[j].blue = color3.b;
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }
                    }

                    rect4.x = (viewRect.width * 0.58f) - 33f;
                    rect4.width = viewRect.width * 0.06f;
                    TooltipHandler.TipRegion(rect4, "Angle, -360 ~ 360");
                    var text = Widgets.TextField(rect4, string.Concat(assignable.flag.patternStack[j].angle));
                    if (IsFullyTypedFloat(text) && float.TryParse(text, out var result) &&
                        assignable.flag.patternStack[j].angle != result)
                    {
                        assignable.flag.patternStack[j].angle = result;
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }
                    }

                    rect4.x = (viewRect.width * 0.46f) - 33f;
                    rect4.width = viewRect.width * 0.06f;
                    TooltipHandler.TipRegion(rect4, "Scale X");
                    text = Widgets.TextField(rect4, string.Concat(assignable.flag.patternStack[j].scaleX * 100f));
                    if (IsFullyTypedFloat(text) && float.TryParse(text, out result) &&
                        Math.Abs((assignable.flag.patternStack[j].scaleX * 100f) - result) > 0.6f)
                    {
                        assignable.flag.patternStack[j].scaleX = result / 100f;
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }
                    }

                    rect4.x = (viewRect.width * 0.52f) - 33f;
                    rect4.width = viewRect.width * 0.06f;
                    TooltipHandler.TipRegion(rect4, "Scale Y");
                    text = Widgets.TextField(rect4, string.Concat(assignable.flag.patternStack[j].scaleY * 100f));
                    if (IsFullyTypedFloat(text) && float.TryParse(text, out result) &&
                        Math.Abs((assignable.flag.patternStack[j].scaleY * 100f) - result) > 0.6f)
                    {
                        assignable.flag.patternStack[j].scaleY = result / 100f;
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }
                    }

                    rect4.x = (viewRect.width * 0.34f) - 33f;
                    rect4.width = viewRect.width * 0.06f;
                    TooltipHandler.TipRegion(rect4, "Offset X");
                    text = Widgets.TextField(rect4, string.Concat(assignable.flag.patternStack[j].offsetX));
                    if (IsFullyTypedFloat(text) && float.TryParse(text, out result) &&
                        assignable.flag.patternStack[j].offsetX != result)
                    {
                        assignable.flag.patternStack[j].offsetX = result;
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }
                    }

                    rect4.x = (viewRect.width * 0.4f) - 33f;
                    rect4.width = viewRect.width * 0.06f;
                    TooltipHandler.TipRegion(rect4, "Offset Y");
                    text = Widgets.TextField(rect4, string.Concat(assignable.flag.patternStack[j].offsetY));
                    if (IsFullyTypedFloat(text) && float.TryParse(text, out result) &&
                        assignable.flag.patternStack[j].offsetY != result)
                    {
                        assignable.flag.patternStack[j].offsetY = result;
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }
                    }

                    rect4.x = (viewRect.width * 0.88f) - 33f;
                    rect4.width = viewRect.width * 0.06f;
                    if (Widgets.ButtonText(rect4, "Amnabi.Up".Translate(), true, false) && j >= 1)
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        Swap(assignable.flag.patternStack, j, j - 1);
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }

                        fiddlerIndex--;
                        return;
                    }

                    rect4.x = (viewRect.width * 0.94f) - 33f;
                    rect4.width = viewRect.width * 0.06f;
                    if (Widgets.ButtonText(rect4, "Amnabi.Down".Translate(), true, false) &&
                        j + 1 < assignable.flag.patternMax)
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        Swap(assignable.flag.patternStack, j, j + 1);
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }

                        fiddlerIndex++;
                        return;
                    }

                    rect4.x = (viewRect.width * 0.28f) - 33f;
                    rect4.width = viewRect.width * 0.06f;
                    if (Widgets.ButtonText(rect4, "Amnabi.Clone".Translate(), true, false))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        if (assignable.flag.patternStack.Count > assignable.flag.patternMax)
                        {
                            assignable.flag.patternStack[assignable.flag.patternMax].customURL =
                                assignable.flag.patternStack[j].customURL;
                            assignable.flag.patternStack[assignable.flag.patternMax].flagPatternDef =
                                assignable.flag.patternStack[j].flagPatternDef;
                            assignable.flag.patternStack[assignable.flag.patternMax].alpha =
                                assignable.flag.patternStack[j].alpha;
                            assignable.flag.patternStack[assignable.flag.patternMax].red =
                                assignable.flag.patternStack[j].red;
                            assignable.flag.patternStack[assignable.flag.patternMax].green =
                                assignable.flag.patternStack[j].green;
                            assignable.flag.patternStack[assignable.flag.patternMax].blue =
                                assignable.flag.patternStack[j].blue;
                            assignable.flag.patternStack[assignable.flag.patternMax].offsetX =
                                assignable.flag.patternStack[j].offsetX;
                            assignable.flag.patternStack[assignable.flag.patternMax].offsetY =
                                assignable.flag.patternStack[j].offsetY;
                            assignable.flag.patternStack[assignable.flag.patternMax].scaleX =
                                assignable.flag.patternStack[j].scaleX;
                            assignable.flag.patternStack[assignable.flag.patternMax].scaleY =
                                assignable.flag.patternStack[j].scaleY;
                            assignable.flag.patternStack[assignable.flag.patternMax].angle =
                                assignable.flag.patternStack[j].angle;
                        }
                        else
                        {
                            assignable.flag.patternStack.Add(
                                new FlagPattern(assignable.flag.patternStack[j].flagPatternDef));
                            assignable.flag.patternStack[assignable.flag.patternMax].customURL =
                                assignable.flag.patternStack[j].customURL;
                            assignable.flag.patternStack[assignable.flag.patternMax].alpha =
                                assignable.flag.patternStack[j].alpha;
                            assignable.flag.patternStack[assignable.flag.patternMax].red =
                                assignable.flag.patternStack[j].red;
                            assignable.flag.patternStack[assignable.flag.patternMax].green =
                                assignable.flag.patternStack[j].green;
                            assignable.flag.patternStack[assignable.flag.patternMax].blue =
                                assignable.flag.patternStack[j].blue;
                            assignable.flag.patternStack[assignable.flag.patternMax].offsetX =
                                assignable.flag.patternStack[j].offsetX;
                            assignable.flag.patternStack[assignable.flag.patternMax].offsetY =
                                assignable.flag.patternStack[j].offsetY;
                            assignable.flag.patternStack[assignable.flag.patternMax].scaleX =
                                assignable.flag.patternStack[j].scaleX;
                            assignable.flag.patternStack[assignable.flag.patternMax].scaleY =
                                assignable.flag.patternStack[j].scaleY;
                            assignable.flag.patternStack[assignable.flag.patternMax].angle =
                                assignable.flag.patternStack[j].angle;
                        }

                        fiddlerIndex = assignable.flag.patternMax;
                        assignable.flag.patternMax++;
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }

                        return;
                    }

                    rect4.x = viewRect.width - 33f;
                    rect4.width = 33f;
                    if (Widgets.ButtonImage(rect4, AmnabiFlagTextures.DeleteX))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        assignable.flag.patternStack.RemoveAt(j);
                        assignable.flag.patternMax--;
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }

                        return;
                    }

                    num9 += 35f;
                }

                num9 += 15f;
            }

            if (assignable.flag != null)
            {
                var num10 = 7;
                var num11 = viewRect.width / num10;
                var num12 = num11 * 0.66f;
                for (var k = 0; k < num10; k++)
                {
                    var rect5 = new Rect(num11 * k, num9, num11, num12);
                    if (Widgets.ButtonImage(rect5, BaseContent.WhiteTex))
                    {
                        assignable.flag.flagShape = (FlagShape)k;
                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }
                    }

                    GUI.color = guiBackground;
                    GUI.DrawTexture(rect5, AmnabiFlagTextures.textureFrom((FlagShape)k));
                    GUI.color = Color.white;
                }

                num9 += 10f + num12;
            }

            var num13 = 4;
            var a = Harmony_Flags.patternByCategory.Keys.Count + 1;
            var num14 = 0;
            var num15 = viewRect.width / Mathf.Min(a, num13);
            foreach (var key in Harmony_Flags.patternByCategory.Keys)
            {
                var rect6 = new Rect(num15 * num14, num9, num15, 32f);
                num14++;
                if (num14 == num13)
                {
                    num14 = 0;
                    num9 += 32f;
                }

                if (!Widgets.ButtonText(rect6, key.Translate(), true, false))
                {
                    continue;
                }

                FloatMenuUtility.MakeMenu(Harmony_Flags.patternByCategory[key], fpd => fpd.defName.Translate(),
                    fpd => delegate
                    {
                        if (assignable.flag == null)
                        {
                            assignable.flag = FlagsCore.CreateFlag(assignable.flagID);
                        }

                        if (assignable.flag != null &&
                            assignable.flag.patternStack.Count > assignable.flag.patternMax)
                        {
                            assignable.flag.patternStack[assignable.flag.patternMax].customURL = "";
                            assignable.flag.patternStack[assignable.flag.patternMax].flagPatternDef = fpd;
                            assignable.flag.patternStack[assignable.flag.patternMax].alpha = 1f;
                            assignable.flag.patternStack[assignable.flag.patternMax].red = 0.5f;
                            assignable.flag.patternStack[assignable.flag.patternMax].green = 0.5f;
                            assignable.flag.patternStack[assignable.flag.patternMax].blue = 0.5f;
                            assignable.flag.patternStack[assignable.flag.patternMax].offsetX =
                                FlagsCore.GLOBALFLAGHEIGHT / 2f;
                            assignable.flag.patternStack[assignable.flag.patternMax].offsetY =
                                FlagsCore.GLOBALFLAGHEIGHT / 2f;
                            assignable.flag.patternStack[assignable.flag.patternMax].scaleX = 1f;
                            assignable.flag.patternStack[assignable.flag.patternMax].scaleY = 1f;
                            assignable.flag.patternStack[assignable.flag.patternMax].angle = 0f;
                        }
                        else
                        {
                            if (assignable.flag != null)
                            {
                                assignable.flag.patternStack.Add(new FlagPattern(fpd));
                                assignable.flag.patternStack[assignable.flag.patternMax].customURL = "";
                                assignable.flag.patternStack[assignable.flag.patternMax].offsetX =
                                    FlagsCore.GLOBALFLAGHEIGHT / 2f;
                                assignable.flag.patternStack[assignable.flag.patternMax].offsetY =
                                    FlagsCore.GLOBALFLAGHEIGHT / 2f;
                            }
                        }

                        if (assignable.flag != null)
                        {
                            fiddlerIndex = assignable.flag.patternMax;
                            assignable.flag.patternMax++;
                        }

                        if (ARM)
                        {
                            needsSave = true;
                        }
                        else
                        {
                            assignable.updateTexture();
                        }
                    });
            }

            if (assignable.flag != null && Widgets.ButtonText(new Rect(num15 * num14, num9, num15, 32f),
                    cantonized ? "Amnabi.Uncantonize".Translate() : "Amnabi.Cantonize".Translate(), true, false))
            {
                if (!cantonized)
                {
                    for (var l = 0; l < assignable.flag.patternMax; l++)
                    {
                        assignable.flag.patternStack[l].offsetX /= 2f;
                        assignable.flag.patternStack[l].offsetY /= 2f;
                        assignable.flag.patternStack[l].offsetY += FlagsCore.GLOBALFLAGHEIGHT / 2f;
                        assignable.flag.patternStack[l].scaleX /= 2f;
                        assignable.flag.patternStack[l].scaleY /= 2f;
                    }

                    assignable.updateTexture();
                }
                else
                {
                    for (var m = 0; m < assignable.flag.patternMax; m++)
                    {
                        assignable.flag.patternStack[m].offsetX *= 2f;
                        assignable.flag.patternStack[m].offsetY -= FlagsCore.GLOBALFLAGHEIGHT / 2f;
                        assignable.flag.patternStack[m].offsetY *= 2f;
                        assignable.flag.patternStack[m].scaleX *= 2f;
                        assignable.flag.patternStack[m].scaleY *= 2f;
                    }

                    assignable.updateTexture();
                }

                cantonized = !cantonized;
            }

            num9 += 32f;
            Widgets.Label(new Rect(0f, num9, viewRect.width, 20f), "Amnabi.PathNameExample".Translate());
            num9 += 20f;
            var num16 = viewRect.width / num13;
            var rect7 = new Rect(num16 * 0f, num9, num16, 32f);
            if (!Widgets.ButtonText(rect7, "Amnabi.ImportPNG".Translate(), true, false))
            {
                return;
            }

            Find.WindowStack.Add(new Dialog_URLInput(assignable));
            if (ARM)
            {
                needsSave = true;
            }
            else
            {
                assignable.updateTexture();
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

    public static bool IsFullyTypedFloat(string str)
    {
        if (str == string.Empty)
        {
            return false;
        }

        var array = str.Split('.');
        return array.Length is <= 2 and >= 1 && ContainsOnlyCharacters(array[0], "-0123456789") &&
               (array.Length != 2 || ContainsOnlyCharacters(array[1], "0123456789"));
    }

    private static bool ContainsOnlyCharacters(string str, string allowedChars)
    {
        foreach (var character in str)
        {
            if (!allowedChars.Contains(character.ToString()))
            {
                return false;
            }
        }

        return true;
    }
}