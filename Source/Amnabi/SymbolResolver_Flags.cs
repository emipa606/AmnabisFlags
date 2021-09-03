using RimWorld.BaseGen;
using Verse;

namespace Amnabi
{
    public class SymbolResolver_Flags : SymbolResolver
    {
        private const float NeverSpawnTorchesIfTemperatureAbove = 18f;

        public override void Resolve(ResolveParams rp)
        {
            var unused = BaseGen.globalSettings.map;
            var aMN_PoleFlagTall = AmnabiFlagDefOfs.AMN_PoleFlagTall;
            if (aMN_PoleFlagTall == null)
            {
                return;
            }

            var resolveParams = rp;
            resolveParams.singleThingDef = aMN_PoleFlagTall;
            resolveParams.postThingSpawn = delegate(Thing t)
            {
                var compFlag = t.TryGetComp<CompFlag>();
                compFlag.TryAssignObject(rp.faction);
            };
            BaseGen.symbolStack.Push("edgeThing", resolveParams);
            BaseGen.symbolStack.Push("edgeThing", resolveParams);
            BaseGen.symbolStack.Push("edgeThing", resolveParams);
        }
    }
}