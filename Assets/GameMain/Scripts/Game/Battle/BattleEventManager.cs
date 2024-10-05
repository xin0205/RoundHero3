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
    }
    
    public class BattleEventData
    {
        public EBattleEventExpressionType BattleEventExpressionType;
        public EBattleEvent BattleEvent;
        public List<List<BattleEventItemData>> BattleGameEventItemDatas = new List<List<BattleEventItemData>>();
        

    }
    
    public class BattleEventManager : Singleton<BattleEventManager>
    {
        public Random Random;
        private int randomSeed;
        
        public void Init(int randomSeed)
        {
            
            this.randomSeed = randomSeed;
            Random = new Random(randomSeed);

        }

        public BattleEventData GenerateEvent()
        {
            var battleGameEventData = new BattleEventData();
            
            var expressions = new List<EBattleEventExpressionType>();
            foreach (var kv in Constant.BattleRandom.EventExpressionTypes)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    expressions.Add(kv.Key);
                }
            }
            
            var eventYNTypes = new List<EBattleEventYNType>();
            foreach (var kv in Constant.BattleRandom.EEventYNTypes)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    eventYNTypes.Add(kv.Key);
                }
            }

            var expressionType = expressions[Random.Next(0, 100)];
            var eventYNType = eventYNTypes[Random.Next(0, 100)];
            
            battleGameEventData.BattleEventExpressionType = expressionType;

            if (expressionType == EBattleEventExpressionType.Game)
            {
                battleGameEventData.BattleEvent  =
                    Constant.BattleRandom.BattleGameEventYNTypes[eventYNType][
                        Random.Next(0, Constant.BattleRandom.BattleGameEventYNTypes[eventYNType].Count)];

            }
            else if (expressionType == EBattleEventExpressionType.Selection)
            {
                battleGameEventData.BattleEvent  =
                    Constant.BattleRandom.BattleSelectEventYNTypes[eventYNType][
                        Random.Next(0, Constant.BattleRandom.BattleSelectEventYNTypes[eventYNType].Count)];

            }
            
            var battleGameEvents = battleGameEventData.BattleEvent.ToString().Split("_");


            for (int i = 1; i < battleGameEvents.Length; i++)
            {
                var battleEventYNType = GameUtility.GetEnum<EBattleEventYNType>(battleGameEvents[i]);
                if (battleEventYNType == EBattleEventYNType.O)
                {
                    battleGameEventData.BattleGameEventItemDatas.Add(new List<BattleEventItemData>());
                }
                else if (battleEventYNType == EBattleEventYNType.YN)
                {

                    var YGameEvent = Constant.BattleRandom.BattleEventYNTypes[EBattleEventYNType.Y][
                        Random.Next(0, Constant.BattleRandom.BattleEventYNTypes[EBattleEventYNType.Y].Count)];
                    var NGameEvents =
                        new List<EEventType>(Constant.BattleRandom.BattleEventYNTypes[EBattleEventYNType.N]);
                    
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
                        Random.Next(0, NGameEvents.Count)];

                    battleGameEventData.BattleGameEventItemDatas.Add(new List<BattleEventItemData>()
                    {
                        new BattleEventItemData()
                        {
                            EventType = YGameEvent,
                            RandomItemIDs = AcquireRandomItemID(YGameEvent),
                        },
                        new BattleEventItemData()
                        {
                            EventType = NGameEvent,
                            RandomItemIDs = AcquireRandomItemID(NGameEvent),
                        },
                    });
                    
                }
                else
                {
                    var gameEvent = Constant.BattleRandom.BattleEventYNTypes[battleEventYNType][
                        Random.Next(0, Constant.BattleRandom.BattleEventYNTypes[battleEventYNType].Count)];
                    battleGameEventData.BattleGameEventItemDatas.Add(new List<BattleEventItemData>()
                    {
                        new BattleEventItemData()
                        {
                            EventType = gameEvent,
                            RandomItemIDs = AcquireRandomItemID(gameEvent),
                        }
                    });

                }

            }

            return battleGameEventData;

        }

        private List<int> AcquireRandomItemID(EEventType eventType)
        {
            var itemIDs = new List<int>();
            if (eventType == EEventType.RandomUnitCard)
            {
                var unitCards = GameEntry.DataTable.GetCards(ECardType.Unit);
                var cardIdxs = MathUtility.GetRandomNum(Constant.BattleRandom.RandomItemCount, 0, unitCards.Length, Random);
                foreach (var cardIdx in cardIdxs)
                {
                    itemIDs.Add(unitCards[cardIdx].Id);
                }

            }
            else if (eventType == EEventType.RandomUnitCard)
            {
                var taticCards = GameEntry.DataTable.GetCards(ECardType.Tactic);
                var cardIdxs = MathUtility.GetRandomNum(Constant.BattleRandom.RandomItemCount, 0, taticCards.Length, Random);
                foreach (var cardIdx in cardIdxs)
                {
                    itemIDs.Add(taticCards[cardIdx].Id);
                }
            }
            else if (eventType == EEventType.NegativeCard)
            {
                var stateCards = GameEntry.DataTable.GetCards(ECardType.State);
                var cardIdxs = MathUtility.GetRandomNum(Constant.BattleRandom.RandomItemCount, 0, stateCards.Length, Random);
                foreach (var cardIdx in cardIdxs)
                {
                    itemIDs.Add(stateCards[cardIdx].Id);
                }
            }
            else if (eventType == EEventType.RandomFune)
            {
                var unitFunes = GameEntry.DataTable.GetDataTable<DRFune>();;
                var funeIdxs = MathUtility.GetRandomNum(Constant.BattleRandom.RandomItemCount, 0, unitFunes.Count, Random);
                foreach (var funeIdx in funeIdxs)
                {
                    itemIDs.Add(unitFunes[funeIdx].Id);
                }
            }
            else if (eventType == EEventType.RandomBless)
            {
                var blesses = GameEntry.DataTable.GetDataTable<DRBless>();;
                var blessIdxs = MathUtility.GetRandomNum(Constant.BattleRandom.RandomItemCount, 0, blesses.Count, Random);
                foreach (var blessIdx in blessIdxs)
                {
                    itemIDs.Add(blesses[blessIdx].Id);
                }
            }

            return itemIDs;
        }
    }
}