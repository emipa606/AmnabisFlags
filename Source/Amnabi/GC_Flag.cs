using Verse;

namespace Amnabi;

public class GC_Flag : GameComponent
{
    public static GC_Flag instance;

    public int instanceID;

    public GC_Flag(Game game)
    {
        instance = this;
        instanceID = Rand.Int;
    }

    public override void ExposeData()
    {
    }
}