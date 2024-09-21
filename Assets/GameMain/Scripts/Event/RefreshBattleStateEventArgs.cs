using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class RefreshBattleStateEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshBattleStateEventArgs).GetHashCode();
        public override int Id => EventId;

        public EBattleState BattleState;
        
        public static RefreshBattleStateEventArgs Create(EBattleState battleState)
        {
            var args = ReferencePool.Acquire<RefreshBattleStateEventArgs>();
            args.BattleState = battleState;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}