using Verse;

namespace Amnabi;

public class CompProperties_Flag : CompProperties
{
    public bool drawOnWall = false;

    public FlagDisplayType FlagDisplayType = FlagDisplayType.Default;

    public string graphicPathStick = "";

    public string graphicPathTop = "";
    public float offsetX;

    public float offsetY;

    public float scaleX = 1f;

    public float scaleY = 1f;

    public CompProperties_Flag()
    {
        compClass = typeof(CompFlag);
    }
}