using RimWorld;

namespace Amnabi;

public class BannerBelt : Apparel
{
    public override void DrawWornExtras()
    {
        base.DrawWornExtras();
        GetComp<CompFlag>().PawnWearDraw();
    }
}