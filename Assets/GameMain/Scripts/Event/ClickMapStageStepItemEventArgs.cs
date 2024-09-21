using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class ClickMapStageStepItemEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ClickMapStageStepItemEventArgs).GetHashCode();
        public override int Id => EventId;

        public EMapSite MapSite;
        public Data_MapStep MapStep;
        
        public static ClickMapStageStepItemEventArgs Create(Data_MapStep mapStep, EMapSite mapSite)
        {
            var args = ReferencePool.Acquire<ClickMapStageStepItemEventArgs>();
            args.MapSite = mapSite;
            args.MapStep = mapStep;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}