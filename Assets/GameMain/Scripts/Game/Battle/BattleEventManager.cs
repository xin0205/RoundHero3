using System;
using System.Collections.Generic;
using System.Linq;
using UGFExtensions.Await;
using UnityEngine.Assertions.Must;


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
        public int RandomSeed;
    }
    
    public class BattleEventData
    {
        public EBattleEventExpressionType BattleEventExpressionType;
        public EBattleEvent BattleEvent;
        public List<List<BattleEventItemData>> BattleEventItemDatas = new List<List<BattleEventItemData>>();
        

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

        public void GenerateEvent(BattleEventData battleEventData, Random random, EBattleEvent battleEvent)
        {
            
            battleEventData.BattleEvent = battleEvent;

            var battleGameEvents = battleEventData.BattleEvent.ToString().Split("_");

            for (int i = 1; i < battleGameEvents.Length;)
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

                if (!CheckSameEvent(battleEventData.BattleEventItemDatas, eventItemDatas))
                {
                    battleEventData.BattleEventItemDatas.Add(eventItemDatas);
                    i++;
                }
                
                // battleEventData.BattleEventItemDatas.Add(eventItemDatas);
                // i++;
            }


        }

        private bool CheckSameEvent(List<List<BattleEventItemData>> allEventItemDatas, List<BattleEventItemData> eventItemDatas)
        {
            foreach (var datas in allEventItemDatas)
            {
                var sameCount = 0;
                foreach (var data in datas)
                {
                    foreach (var data2 in eventItemDatas)
                    {
                        if (data.EventType == data2.EventType)
                        {
                            sameCount += 1;
                        }
                    }
                }

                if (sameCount == datas.Count)
                {
                    return true;
                }
            }

            return false;
        }
        
        public BattleEventData GenerateRandomEvent(int randomSeed)
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

            EBattleEvent battleEvent = EBattleEvent.Select_Y_Y_O; 
            
            if (expressionType == EBattleEventExpressionType.Game)
            {
                battleEvent  =
                    Constant.BattleEvent.BattleGameEventYNTypes[eventYNType][
                        random.Next(0, Constant.BattleEvent.BattleGameEventYNTypes[eventYNType].Count)];
            
            }
            else if (expressionType == EBattleEventExpressionType.Selection)
            {
                battleEvent  =
                    Constant.BattleEvent.BattleSelectEventYNTypes[eventYNType][
                        random.Next(0, Constant.BattleEvent.BattleSelectEventYNTypes[eventYNType].Count)];
            
            }


            GenerateEvent(battleGameEventData, random, battleEvent);
            

            return battleGameEventData;

        }
        
        public BattleEventData GenerateTreasureEvent(int randomSeed)
        {
            var random = new Random(randomSeed);
            
            var battleGameEventData = new BattleEventData();

            var keys = Constant.BattleEvent.BattleTreasureTypes.Keys.ToList();
            var eventYNType = keys[random.Next(0, keys.Count)];

            EBattleEvent battleEvent = Constant.BattleEvent.BattleTreasureTypes[eventYNType][
                random.Next(0, Constant.BattleEvent.BattleSelectEventYNTypes[eventYNType].Count)]; 
            
            


            GenerateEvent(battleGameEventData, random, battleEvent);
            

            return battleGameEventData;

        }

        public BattleEventItemData AcquireBattleEventItemData(EBattleEventYNType battleEventYNType, EEventType eventType, Random random)
        {
            var battleEventItemData = new BattleEventItemData()
            {
                EventType = eventType,
                RandomSeed = random.Next(0, Constant.Game.RandomRange),
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

        
        public void AcquireEventItem(BattleEventItemData battleEventItemData, int selectIdx = 0)
        {
            var eventType = battleEventItemData.EventType;
            var eventValue = 0;
            // if (battleEventItemData.EventValues.Contains(selectIdx))
            // {
            //     eventValue = battleEventItemData.EventValues[selectIdx];
            // }
            
            if (eventType == EEventType.Appoint_UnitCard || eventType == EEventType.Appoint_TacticCard ||eventType == EEventType.NegativeCard)
            {
                var cardIdx = CardManager.Instance.GetIdx();
                eventValue = battleEventItemData.EventValues[0];
                CardManager.Instance.CardDatas.Add(cardIdx, new Data_Card(cardIdx, eventValue));
            }
            else if (eventType == EEventType.Random_UnitCard || eventType == EEventType.Random_TacticCard)
            {
                var cardIdx = CardManager.Instance.GetIdx();
                eventValue = battleEventItemData.EventValues[selectIdx];
                CardManager.Instance.CardDatas.Add(cardIdx, new Data_Card(cardIdx, eventValue));
            }
            else if (eventType == EEventType.Random_Fune)
            {
                var funeIdx = FuneManager.Instance.GetIdx();
                eventValue = battleEventItemData.EventValues[selectIdx];
                var drFune = GameEntry.DataTable.GetBuff(eventValue);
                var value = drFune == null ? 0 : BattleBuffManager.Instance.GetBuffValue(drFune.GetValues(0)[0]);
                
                FuneManager.Instance.FuneDatas.Add(funeIdx,new Data_Fune(funeIdx, (int)value));
                BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs.Add(funeIdx);
            }
            else if (eventType == EEventType.Appoint_Fune)
            {
                var funeIdx = FuneManager.Instance.GetIdx();
                eventValue = battleEventItemData.EventValues[0];
                var drFune = GameEntry.DataTable.GetBuff(eventValue);
                var value = drFune == null ? 0 : BattleBuffManager.Instance.GetBuffValue(drFune.GetValues(0)[0]);
                
                FuneManager.Instance.FuneDatas.Add(funeIdx,new Data_Fune(funeIdx, (int)value));
                BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs.Add(funeIdx);
            }
            else if (eventType == EEventType.Random_Bless)
            {
                var blessIdx = BlessManager.Instance.GetIdx();
                eventValue = battleEventItemData.EventValues[selectIdx];
                var drBless = GameEntry.DataTable.GetBless(eventValue);
                
                BlessManager.Instance.BlessDatas.Add(blessIdx,new Data_Bless(blessIdx, drBless.BlessID));

            }
            else if (eventType == EEventType.Appoint_Bless)
            {
                var blessIdx = BlessManager.Instance.GetIdx();
                eventValue = battleEventItemData.EventValues[0];
                var drBless = GameEntry.DataTable.GetBless(eventValue);
                
                BlessManager.Instance.BlessDatas.Add(blessIdx,new Data_Bless(blessIdx, drBless.BlessID));

            }
            else if (eventType == EEventType.AddCoin || eventType == EEventType.SubCoin)
            {
                eventValue = battleEventItemData.EventValues[0];
                PlayerManager.Instance.PlayerData.Coin += eventValue;
            }
            else if (eventType == EEventType.AddHeroMaxHP || eventType == EEventType.SubHeroMaxHP)
            {
                eventValue = battleEventItemData.EventValues[0];
                PlayerManager.Instance.PlayerData.BattleHero.BaseMaxHP += eventValue;
            }
            else if (eventType == EEventType.AddHeroCurHP || eventType == EEventType.SubHeroCurHP)
            {
                eventValue = battleEventItemData.EventValues[0];
                PlayerManager.Instance.PlayerData.BattleHero.CurHP += eventValue;
            }
            else if(eventType == EEventType.Card_Copy)
            {
                eventValue = battleEventItemData.EventValues[0];
                var cardData = CardManager.Instance.GetCard(eventValue);
                var newCardIdx = PlayerManager.Instance.PlayerData.CardIdx++;
                //battleEventItemData.EventValues.Add(cardData.CardID);
                CardManager.Instance.CardDatas.Add(newCardIdx, new Data_Card(newCardIdx, cardData.CardID));
                
                
                
            }
            else if(eventType == EEventType.Card_Remove)
            {
                eventValue = battleEventItemData.EventValues[0];
                var cardData = CardManager.Instance.GetCard(eventValue);
                //battleEventItemData.EventValues.Add(cardData.CardIdx);
                CardManager.Instance.CardDatas.Remove(cardData.CardIdx);
                
                
                
            }
            else if(eventType == EEventType.Card_Change)
            {
                eventValue = battleEventItemData.EventValues[0];
                var cardData = CardManager.Instance.GetCard(eventValue);
                CardManager.Instance.CardDatas.Remove(cardData.CardIdx);
                
                var newCardIdx = PlayerManager.Instance.PlayerData.CardIdx++;
                
                var cards = GameEntry.DataTable.GetCards(new List<ECardType>() { ECardType.Tactic }, cardData.CardID);
                var random = new Random(battleEventItemData.RandomSeed);
                
                var randomIdxs = MathUtility.GetRandomNum(1, 0, cards.Length, random);
                battleEventItemData.EventValues[0] = cards[randomIdxs[0]].Id;
                CardManager.Instance.CardDatas.Add(newCardIdx, new Data_Card(newCardIdx, cards[randomIdxs[0]].Id));
                
            }
            
            GameEntry.Event.Fire(null, RefreshPlayerInfoEventArgs.Create());
        }
        
        
    }
}