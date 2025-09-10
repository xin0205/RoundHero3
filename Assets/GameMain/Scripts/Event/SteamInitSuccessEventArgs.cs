using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class SteamInitSuccessEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(SteamInitSuccessEventArgs).GetHashCode();
        public override int Id => EventId;

        public static SteamInitSuccessEventArgs Create()
        {
            var args = ReferencePool.Acquire<SteamInitSuccessEventArgs>();
            return args;
        }

        public override void Clear()
        {

        }
    }
}