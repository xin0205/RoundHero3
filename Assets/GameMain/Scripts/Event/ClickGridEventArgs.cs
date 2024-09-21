using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class ClickGridEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ClickGridEventArgs).GetHashCode();
        public override int Id => EventId;

        public int GridPosIdx;

        
        public static ClickGridEventArgs Create(int posIdx)
        {
            var args = ReferencePool.Acquire<ClickGridEventArgs>();
            args.GridPosIdx = posIdx;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}