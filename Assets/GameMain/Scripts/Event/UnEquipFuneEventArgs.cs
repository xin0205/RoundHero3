using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class UnEquipFuneEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UnEquipFuneEventArgs).GetHashCode();
        public override int Id => EventId;

        public int FuneIdx;

        
        public static UnEquipFuneEventArgs Create(int funeIdx)
        {
            var args = ReferencePool.Acquire<UnEquipFuneEventArgs>();
            args.FuneIdx = funeIdx;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}