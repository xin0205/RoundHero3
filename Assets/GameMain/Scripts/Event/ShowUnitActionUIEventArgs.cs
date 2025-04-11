

using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class ShowUnitActionUIEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ShowUnitActionUIEventArgs).GetHashCode();
        public override int Id => EventId;

        public bool IsShow;
        
        public Vector3 UnitPosition;

        public static ShowUnitActionUIEventArgs Create(bool isShow, Vector3 unitPosition)
        {
            var args = ReferencePool.Acquire<ShowUnitActionUIEventArgs>();
            args.IsShow = isShow;
            args.UnitPosition = unitPosition;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}