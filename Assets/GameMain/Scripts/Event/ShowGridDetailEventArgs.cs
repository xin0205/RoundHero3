using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class ShowGridDetailEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ShowGridDetailEventArgs).GetHashCode();
        public override int Id => EventId;

        public int GridPosIdx;
        
        public EShowState ShowState;
        
        public static ShowGridDetailEventArgs Create(int posIdx, EShowState showState = EShowState.Keep)
        {
            var args = ReferencePool.Acquire<ShowGridDetailEventArgs>();
            args.GridPosIdx = posIdx;
            args.ShowState = showState;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}