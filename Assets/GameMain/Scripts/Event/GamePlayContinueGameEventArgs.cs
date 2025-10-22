using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class GamePlayContinueGameEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GamePlayContinueGameEventArgs).GetHashCode();
        public override int Id => EventId;

        
       
        public static GamePlayContinueGameEventArgs Create()
        {
            var args = ReferencePool.Acquire<GamePlayContinueGameEventArgs>();
            
            return args;
        }
        
        
        
        public override void Clear()
        {

        }
    }
}