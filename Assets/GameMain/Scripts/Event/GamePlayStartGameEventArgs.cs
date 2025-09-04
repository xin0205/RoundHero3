using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class GamePlayStartGameEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GamePlayStartGameEventArgs).GetHashCode();
        public override int Id => EventId;

        
       
        public static GamePlayStartGameEventArgs Create()
        {
            var args = ReferencePool.Acquire<GamePlayStartGameEventArgs>();
            
            return args;
        }
        
        
        
        public override void Clear()
        {

        }
    }
}