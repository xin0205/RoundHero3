using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class StartSelect_SelectHeroEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(StartSelect_SelectHeroEventArgs).GetHashCode();
        public override int Id => EventId;

        public int HeroID;

        
        public static StartSelect_SelectHeroEventArgs Create(int heroID)
        {
            var args = ReferencePool.Acquire<StartSelect_SelectHeroEventArgs>();
            args.HeroID = heroID;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}