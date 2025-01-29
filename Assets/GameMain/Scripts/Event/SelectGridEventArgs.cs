using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class SelectGridEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(SelectGridEventArgs).GetHashCode();
        public override int Id => EventId;

        public int GridPosIdx;

        public bool IsSelect = false;

        
        public static SelectGridEventArgs Create(int posIdx, bool isSelect)
        {
            var args = ReferencePool.Acquire<SelectGridEventArgs>();
            args.GridPosIdx = posIdx;
            args.IsSelect = isSelect;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}