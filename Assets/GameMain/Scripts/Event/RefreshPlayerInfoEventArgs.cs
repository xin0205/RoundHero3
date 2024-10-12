using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class RefreshPlayerInfoEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshPlayerInfoEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public static RefreshPlayerInfoEventArgs Create()
        {
            var args = ReferencePool.Acquire<RefreshPlayerInfoEventArgs>();
      
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}