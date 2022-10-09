using RimWorld.BaseGen;
using Verse;

namespace Amnabi;

public class SymbolResolver_Flags : SymbolResolver
{
    private const float NeverSpawnTorchesIfTemperatureAbove = 18f;

    public override void Resolve(ResolveParams rp)
    {
        var unused = BaseGen.globalSettings.map;
        var singleThingDef = AmnabiFlagDefOfs.AMN_PoleFlagTall;
        if (singleThingDef == null)
        {
            return;
        }

        var resolveParams = rp;
        resolveParams.singleThingDef = singleThingDef;
        resolveParams.postThingSpawn = delegate(Thing t) { t.TryGetComp<CompFlag>().TryAssignObject(rp.faction); };
        BaseGen.symbolStack.Push("edgeThing", resolveParams);
        BaseGen.symbolStack.Push("edgeThing", resolveParams);
        BaseGen.symbolStack.Push("edgeThing", resolveParams);
    }
}