using System;
using System.Collections.Generic;


namespace RoundHero
{
    // public class BattleEventData
    // {
    
    //
    // }

    public class BattleEventItemData
    {
        public EEventType EventType;
        public List<int> RandomItemIDs = new List<int>();
        public List<int> EventValues = new List<int>();
    }
    
    public class BattleEventData
    {
        public EBattleEventExpressionType BattleEventExpressionType;
        public EBattleEvent BattleEvent;
        public List<List<BattleEventItemData>> BattleGameEventItemDatas = new List<List<BattleEventItemData>>();
        

    }
    
    public class BattleEventManager : Singleton<BattleEventManager>
    {
        // public Random Random;
        // private int randomSeed;
        //
        // public void Init(int randomSeed)
        // {
        //     
        //     this.randomSeed = randomSeed;
        //     Random = new Random(randomSeed);
        //
        // }

        public BattleEventData GenerateEvent(int randomSeed)
        {
            var random = new Random(randomSeed);
            
            var battleGameEventData = new BattleEventData();
            
            var expressions = new List<EBattleEventExpressionType>();
            foreach (var kv in Constant.BattleEvent.EventExpressionTypes)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    expressions.Add(kv.Key);
                }
            }
            
            var eventYNTypes = new List<EBattleEventYNType>();
            foreach (var kv in Constant.BattleEvent.EEventYNTypes)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    eventYNTypes.Add(kv.Key);
                }
            }

            var expressionType = expressions[random.Next(0, 100)];
            var eventYNType = eventYNTypes[random.Next(0, 100)];
            
            battleGameEventData.BattleEventExpressionType = expressionType;

            if (expressionType == EBattleEventExpressionType.Game)
            {
                battleGameEventData.BattleEvent  =
                    Constant.BattleEvent.BattleGameEventYNTypes[eventYNType][
                        random.Next(0, Constant.BattleEvent.BattleGameEventYNTypes[eventYNType].Count)];

            }
            else if (expressionType == EBattleEventExpressionType.Selection)
            {
                battleGameEventData.BattleEvent  =
                    Constant.BattleEvent.BattleSelectEventYNTypes[eventYNType][
                        random.Next(0, Constant.BattleEvent.BattleSelectEventYNTypes[eventYNType].Count)];

            }
            
            var battleGameEvents = battleGameEventData.BattleEvent.ToString().Split("_");


            for (int i = 1; i < battleGameEvents.Length; i++)
            {
                var eventItemDatas = new List<BattleEventItemData>();
                var battleEventYNType = GameUtility.GetEnum<EBattleEventYNType>(battleGameEvents[i]);
                if (battleEventYNType == EBattleEventYNType.O)
                {
                    
                }
                else if (battleEventYNType == EBattleEventYNType.YN)
                {
                    var YGameEvent = Constant.BattleEvent.BattleEventYNTypes[EBattleEventYNType.Y][
                        random.Next(0, Constant.BattleEvent.BattleEventYNTypes[EBattleEventYNType.Y].Count)];
                    var NGameEvents =
                        new List<EEventType>(Constant.BattleEvent.BattleEventYNTypes[EBattleEventYNType.N]);
                    
                    if (YGameEvent == EEventType.AddCoin)
                    {
                        NGameEvents.Remove(EEventType.SubCoin);
                    }
                    else if (YGameEvent == EEventType.AddHeroCurHP)
                    {
                        NGameEvents.Remove(EEventType.SubHeroCurHP);
                    }
                    else if (YGameEvent == EEventType.AddHeroMaxHP)
                    {
                        NGameEvents.Remove(EEventType.SubHeroMaxHP);
                    }

                    var NGameEvent = NGameEvents[
                        random.Next(0, NGameEvents.Count)];

                    
                    eventItemDatas.Add(AcquireBattleEventItemData(battleEventYNType, YGameEvent, random));
                    eventItemDatas.Add(AcquireBattleEventItemData(battleEventYNType, NGameEvent, random));
                    
                }
                else
                {
                    var gameEvent = Constant.BattleEvent.BattleEventYNTypes[battleEventYNType][
                        random.Next(0, Constant.BattleEvent.BattleEventYNTypes[battleEventYNType].Count)];
                    
                    eventItemDatas.Add(AcquireBattleEventItemData(battleEventYNType, gameEvent, random));
                    
                }
                
                battleGameEventData.BattleGameEventItemDatas.Add(eventItemDatas);

            }

            return battleGameEventData;

        }

        private BattleEventItemData AcquireBattleEventItemData(EBattleEventYNType battleEventYNType, EEventType eventType, Random random)
        {
            var battleEventItemData = new BattleEventItemData()
            {
                EventType = eventType,
            };
            
            if (Constant.BattleEvent.BattleEventSubTypes.ContainsKey(EEventSubType.Random))
            {
                battleEventItemData.EventValues = AcquireRandomItemID(eventType, random, Constant.BattleEvent.RandomItemCount);
            }
            else if (Constant.BattleEvent.BattleEventSubTypes.ContainsKey(EEventSubType.Appoint))
            {
                battleEventItemData.EventValues = AcquireRandomItemID(eventType, random, 1);
            }
            else if (Constant.BattleEvent.BattleEventSubTypes.ContainsKey(EEventSubType.Value))
            {
                var valueRange = Constant.BattleEvent.BattleEventValues[battleEventYNType][eventType];
                battleEventItemData.EventValues.Add(random.Next(valueRange.x, valueRange.y));
            }

            return battleEventItemData;
        }

        private List<int> AcquireRandomItemID(EEventType eventType, Random random, int randomCount)
        {
            var itemIDs = new List<int>();
            if (eventType == EEventType.Random_UnitCard || eventType == EEventType.Appoint_UnitCard)
            {
                var unitCards = GameEntry.DataTable.GetCards(ECardType.Unit);
                var cardIdxs = MathUtility.GetRandomNum(Constant.BattleEvent.RandomItemCount, 0, unitCards.Length, random);
                foreach (var cardIdx in cardIdxs)
                {
                    itemIDs.Add(unitCards[cardIdx].Id);
                }

            }
            else if (eventType == EEventType.Random_TacticCard || eventType == EEventType.Appoint_TacticCard)
            {
                var taticCards = GameEntry.DataTable.GetCards(ECardType.Tactic);
                var cardIdxs = MathUtility.GetRandomNum(randomCount, 0, taticCards.Length, random);
                foreach (var cardIdx in cardIdxs)
                {
                    itemIDs.Add(taticCards[cardIdx].Id);
                }
            }
            else if (eventType == EEventType.NegativeCard)
            {
                var stateCards = GameEntry.DataTable.GetCards(ECardType.State);
                var cardIdxs = MathUtility.GetRandomNum(randomCount, 0, stateCards.Length, random);
                foreach (var cardIdx in cardIdxs)
                {
                    itemIDs.Add(stateCards[cardIdx].Id);
                }
            }
            else if (eventType == EEventType.Random_Fune || eventType == EEventType.Appoint_Fune)
            {
                var unitFunes = GameEntry.DataTable.GetDataTable<DRFune>();;
                var funeIdxs = MathUtility.GetRandomNum(randomCount, 0, unitFunes.Count, random);
                foreach (var funeIdx in funeIdxs)
                {
                    itemIDs.Add(unitFunes[funeIdx].Id);
                }
            }
            else if (eventType == EEventType.Random_Bless || eventType == EEventType.Appoint_Bless)
            {
                var blesses = GameEntry.DataTable.GetDataTable<DRBless>();;
                var blessIdxs = MathUtility.GetRandomNum(randomCount, 0, blesses.Count, random);
                foreach (var blessIdx in blessIdxs)
                {
                    itemIDs.Add(blesses[blessIdx].Id);
                }
            }

            return itemIDs;
        }
    }
}