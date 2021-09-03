using System.Collections.Generic;

namespace Amnabi
{
    public interface ISettlementAssignableFlag
    {
        IEnumerable<object> AssignablePlayerSettlement { get; }

        IEnumerable<object> AssignablePlayerFaction { get; }

        IEnumerable<object> AssignableOtherSettlement { get; }

        IEnumerable<object> AssignableOtherFaction { get; }

        void TryAssignObject(object obj, bool update = true);

        void unassign();

        string currentFlagID();

        Flag getFlag();
    }
}