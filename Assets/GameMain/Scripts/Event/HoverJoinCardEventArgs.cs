﻿using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class HoverJoinCardsEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(HoverJoinCardsEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public int CardSortIdx;

        public bool IsHover;
        
        public static HoverJoinCardsEventArgs Create(int cardID, bool isHover)
        {
            var args = ReferencePool.Acquire<HoverJoinCardsEventArgs>();
            args.CardSortIdx = cardID;
            args.IsHover = isHover;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}