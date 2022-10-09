using Verse;

namespace Amnabi;

public class PlaceWorker_WallFlag : PlaceWorker
{
    public new virtual AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thing = null)
    {
        return !loc.Fogged(map) && loc.Fogged(map) && (loc + rot.FacingCell).InBounds(map) &&
               !(loc + rot.FacingCell).Fogged(map);
    }
}