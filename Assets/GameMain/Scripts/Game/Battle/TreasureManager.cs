using System;
using System.Collections.Generic;

namespace RoundHero
{
    public class TreasureManager : Singleton<TreasureManager>
    {
        public List<BattleEventItemData> GenerateTreasureEvent(int randomSeed)
        {
            var eventItemDatas = new List<BattleEventItemData>();
            
            var random = new Random(randomSeed);
            
            var gameEvent = Constant.BattleEvent.BattleEventYNTypes[EBattleEventYNType.Y][
                random.Next(0, Constant.BattleEvent.BattleEventYNTypes[EBattleEventYNType.Y].Count)];
                    
            eventItemDatas.Add(BattleEventManager.Instance.AcquireBattleEventItemData(EBattleEventYNType.Y, gameEvent, random));

            return eventItemDatas;
        }
    }
}