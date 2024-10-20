﻿using System;
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
        //public List<int> RandomItemIDs = new List<int>();
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
                    var YGameEvent = Constant.BattleEvent.BattleEventYNTypes2[EBattleEventYNType.Y][
                        random.Next(0, Constant.BattleEvent.BattleEventYNTypes2[EBattleEventYNType.Y].Count)];
                    var NGameEvents =
                        new List<EEventType>(Constant.BattleEvent.BattleEventYNTypes2[EBattleEventYNType.N]);
                    
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

        public BattleEventItemData AcquireBattleEventItemData(EBattleEventYNType battleEventYNType, EEventType eventType, Random random)
        {
            var battleEventItemData = new BattleEventItemData()
            {
                EventType = eventType,
            };
            
            if (Constant.BattleEvent.BattleEventSubTypes[EEventSubType.Random].Contains(eventType))
            {
                battleEventItemData.EventValues = AcquireRandomItemID(eventType, random, Constant.BattleEvent.RandomItemCount);
            }
            else if (Constant.BattleEvent.BattleEventSubTypes[EEventSubType.Appoint].Contains(eventType))
            {
                battleEventItemData.EventValues = AcquireRandomItemID(eventType, random, 1);
            }
            else if (eventType == EEventType.NegativeCard)
            {
                battleEventItemData.EventValues = AcquireRandomItemID(eventType, random, 1);
            }
            else if (Constant.BattleEvent.BattleEventSubTypes[EEventSubType.Value].Contains(eventType))
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
                var cardIdxs = MathUtility.GetRandomNum(randomCount, 0, unitCards.Length, random);
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
                var unitFunes = GameEntry.DataTable.GetBuffs(EBuffType.Fune);
                var funeIdxs = MathUtility.GetRandomNum(randomCount, 0, unitFunes.Length, random);
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


        public void AcquireEventItem(BattleEventItemData battleEventItemData, int selectIdx = 0)
        {
            var eventType = battleEventItemData.EventType;
            var eventValue = battleEventItemData.EventValues[selectIdx];
            if (eventType == EEventType.Appoint_UnitCard || eventType == EEventType.Appoint_TacticCard ||eventType == EEventType.NegativeCard ||
                eventType == EEventType.Random_UnitCard || eventType == EEventType.Random_TacticCard)
            {
                var cardIdx = PlayerManager.Instance.PlayerData.CardIdx++;
                CardManager.Instance.CardDatas.Add(cardIdx, new Data_Card(cardIdx, eventValue));
            }
            else if (eventType == EEventType.Random_Fune || eventType == EEventType.Appoint_Fune)
            {
                var funeIdx = PlayerManager.Instance.PlayerData.FuneIdx++;
                var drFune = GameEntry.DataTable.GetBuff(eventValue);
                var value = drFune == null ? 0 : BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[0]);
                
                FuneManager.Instance.FuneDatas.Add(funeIdx,new Data_Fune(funeIdx, value));
                BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs.Add(funeIdx);
            }
            else if (eventType == EEventType.Random_Bless || eventType == EEventType.Appoint_Bless)
            {
                var blessIdx = PlayerManager.Instance.PlayerData.BlessIdx++;
                BlessManager.Instance.BlessDatas.Add(blessIdx,new Data_Bless(blessIdx, eventValue));

            }
            else if (eventType == EEventType.AddCoin || eventType == EEventType.SubCoin)
            {
                PlayerManager.Instance.PlayerData.Coin += eventValue;
            }
            else if (eventType == EEventType.AddHeroMaxHP || eventType == EEventType.SubHeroMaxHP)
            {
                PlayerManager.Instance.PlayerData.BattleHero.BaseMaxHP += eventValue;
            }
            else if (eventType == EEventType.AddHeroCurHP || eventType == EEventType.SubHeroCurHP)
            {
                PlayerManager.Instance.PlayerData.BattleHero.CurHP += eventValue;
            }
        }
    }
}