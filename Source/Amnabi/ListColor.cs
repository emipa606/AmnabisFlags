using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Amnabi
{
    public class ListColor : IExposable
    {
        public List<Color> internalList = new List<Color>();

        public virtual void ExposeData()
        {
            Scribe_Collections.Look(ref internalList, "internalList", LookMode.Value);
        }
    }
}