using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{

    public class SwitchActionCampEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(SwitchActionCampEventArgs).GetHashCode();
        public override int Id => EventId;

        public EUnitCamp UnitCamp;

        
        public static SwitchActionCampEventArgs Create(EUnitCamp unitCamp)
        {
            var args = ReferencePool.Acquire<SwitchActionCampEventArgs>();
            args.UnitCamp = unitCamp;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}