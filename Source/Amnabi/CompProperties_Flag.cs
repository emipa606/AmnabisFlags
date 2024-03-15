using Verse;

namespace Amnabi;

public class CompProperties_Flag : CompProperties
{
    public readonly bool drawOnWall = false;

    public readonly FlagDisplayType FlagDisplayType = FlagDisplayType.Default;

    public readonly string graphicPathStick = "";

    public readonly string graphicPathTop = "";

    public readonly float scaleX = 1f;

    public readonly float scaleY = 1f;
    public float offsetX;

    public float offsetY;

    public CompProperties_Flag()
    {
        compClass = typeof(CompFlag);
    }
}