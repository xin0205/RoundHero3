using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class RefreshCardsFormEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshCardsFormEventArgs).GetHashCode();
        public override int Id => EventId;

        
        public static RefreshCardsFormEventArgs Create()
        {
            var args = ReferencePool.Acquire<RefreshCardsFormEventArgs>();
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}